using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using OnlineClassifieds.Services;
using OnlineClassifieds.Helpers;
using OnlineClassifieds.ViewModels;

using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.Models;

namespace OnlineClassifieds.Controllers
{
    public class AnnouncementController : Controller
    {
        private const int PageSize = 5;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly FilesWorkService _filesWorkService;
        private readonly CurrentUserProvider _currentUserProvider;
        private readonly ICookieService _cookieService;

        public AnnouncementController(
            IAnnouncementRepository announcementRepository,
            FilesWorkService filesWorkService,
            CurrentUserProvider currentUserProvider,
            ICookieService cookieService)
        {
            _announcementRepository = announcementRepository;
            _filesWorkService = filesWorkService;
            _currentUserProvider = currentUserProvider;
            _cookieService = cookieService;
        }


        private async Task<AnnouncementVM> GetAnnouncementVM(
            string cookieIdCat = "-1",
            int numPage = 1,
            string? titleFilter = null)
        {
            IEnumerable<Announcement> announcements;
            Func<IQueryable<Announcement>, IOrderedQueryable<Announcement>> orderDateByDesc = 
                q => q.OrderByDescending(announ => announ.CreateDt);
            if (cookieIdCat.Equals("-1"))
            {
                announcements = await _announcementRepository.GetAll(
                    announ => announ.Title.Contains(titleFilter ?? ""),
                    orderDateByDesc,
                    includeProps: "Category,User"
                );
            }
            else
            {
                announcements = await _announcementRepository.GetAll(
                    announ => announ.Category.Id.ToString().Equals(cookieIdCat),
                    orderDateByDesc,
                    includeProps: "Category,User"
                );
            }
            return new AnnouncementVM()
            {
                Announcements = announcements,
                Pager = new Pager(announcements.Count(), numPage, PageSize)
            };
        }

        public async Task<IActionResult> Index()
        {
            // получаем выбранную категорию и поисковую строку
            string cookieIdCat = _cookieService.GetCookie(WC.CookieCategoryIdKey) ?? "-1";
            string cookieTitleSearch = _cookieService.GetCookie(WC.CookieSearchTitleKey) ?? String.Empty;
            return View(await GetAnnouncementVM(cookieIdCat, 1, cookieTitleSearch));
        }

        [HttpPost]
        public async Task<IActionResult> SearchByCategory(string idCat = "-1")
        {
            _cookieService.SetCookie(WC.CookieSearchTitleKey, String.Empty);  // обнуляем данные о поисковой строке
            _cookieService.SetCookie(WC.CookieCategoryIdKey, idCat, DateTimeOffset.UtcNow.AddMinutes(1));
            return PartialView("_AnnouncementList", await GetAnnouncementVM(idCat));
        }

        [HttpPost]
        public async Task<IActionResult> SwitchPage(int numPage = 1)
        {
            // получаем выбранную категорию и поисковую строку
            if (numPage < 1) { numPage = 1; }
            string cookieIdCat = _cookieService.GetCookie(WC.CookieCategoryIdKey) ?? "-1";
            string cookieTitleSearch = _cookieService.GetCookie(WC.CookieSearchTitleKey) ?? String.Empty;
            return PartialView("_AnnouncementList", await GetAnnouncementVM(cookieIdCat, numPage, cookieTitleSearch));
        }

        [HttpPost]
        public async Task<IActionResult> SearchByText(string _titleFilter)
        {
            _cookieService.SetCookie(WC.CookieCategoryIdKey, "-1");  // обнуляем данные о выбранной категории
            _cookieService.SetCookie(WC.CookieSearchTitleKey, _titleFilter, DateTimeOffset.UtcNow.AddMinutes(1));
            return PartialView("_AnnouncementList", await GetAnnouncementVM(titleFilter: _titleFilter));
        }


        [Authorize]
        public IActionResult Create()
        {
            ViewData["CategoryItems"] = _announcementRepository.GetAllDropDownList("Category");
            return View();
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Announcement announcement)
        {
            if ((announcement.Category is not null || announcement.User is not null) && !ModelState.IsValid)
            {
                return RedirectToAction(nameof(Create));
            }

            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                string newFilename = await _filesWorkService.DownloadFileForm(WC.ImageAnnouncementPath, files[0]);
                announcement.Image = newFilename;
            }

            announcement.IdUser = _currentUserProvider.GetCurrentUserId();
            await _announcementRepository.Add(announcement);
            await _announcementRepository.Save();

            return Redirect(nameof(GetUserAnnouncement));
        }

        [Authorize]
        public async Task<IActionResult> GetUserAnnouncement()
        {
            string? userId = _currentUserProvider.GetCurrentUserId();
            if (userId is null) { return NotFound(); }

            Func<IQueryable<Announcement>, IOrderedQueryable<Announcement>> orderDateByDesc =
                q => q.OrderByDescending(announ => announ.CreateDt);
            var announcements = await _announcementRepository.GetAll(
                a => a.IdUser!.Equals(userId),
                orderDateByDesc,
                includeProps: "Category,User"
            );

            return View(announcements);
        }
    }
}
