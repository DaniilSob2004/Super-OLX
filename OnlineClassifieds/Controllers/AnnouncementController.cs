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
        private readonly CacheService<IEnumerable<Announcement>> _cacheService;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly FilesWorkService _filesWorkService;
        private readonly CurrentUserProvider _currentUserProvider;
        private readonly ICookieService _cookieService;

        public AnnouncementController(
            CacheService<IEnumerable<Announcement>> cacheService,
            IAnnouncementRepository announcementRepository,
            FilesWorkService filesWorkService,
            CurrentUserProvider currentUserProvider,
            ICookieService cookieService)
        {
            _cacheService = cacheService;
            _announcementRepository = announcementRepository;
            _filesWorkService = filesWorkService;
            _currentUserProvider = currentUserProvider;
            _cookieService = cookieService;
        }


        private async Task UpdateCacheAllAnnouncement()
        {
            // обновление кэша (будет обращаться к бд)
            await _cacheService.Create(
                WC.CacheAllAnnouncementKey,
                async () => await _announcementRepository.GetAll(
                    a => a.IsActive,
                    includeProps: "Category,User"
                )
            );
        }


        private async Task ChangeStateAnnouncement(string id, bool isActivate)
        {
            if (!Guid.TryParse(id, out Guid announId)) { return; }

            if (isActivate) { await _announcementRepository.Activate(announId); }
            else { await _announcementRepository.Deactivate(announId); }
            
            await _announcementRepository.Save();

            await UpdateCacheAllAnnouncement();  // обновляем кэш
        }


        private async Task<AnnouncementVM> GetAnnouncementVM(
            string idCat = "-1",
            string titleSearch = "",
            int numPage = 1,
            string? sortBy = null,
            string isAscending = "true")
        {
            // получаем объявления из кэша
            IEnumerable<Announcement> announcements = await _cacheService.GetOrCreate(
                WC.CacheAllAnnouncementKey,
                async () => await _announcementRepository.GetAll(
                    a => a.IsActive,
                    includeProps: "Category,User"
                )
            );

            // объект сортировки
            Func<IQueryable<Announcement>, IOrderedQueryable<Announcement>>? sorting = null;
            if (sortBy is not null)
            {
                switch (sortBy)
                {
                    case WC.SortByField_Reset:
                        if (!isAscending.Equals("")) { sorting = q => q.OrderBy(announ => announ.CreateDt); }
                        else { sorting = q => q.OrderByDescending(announ => announ.CreateDt); }
                        break;

                    case WC.SortByField_Price:
                        if (!isAscending.Equals("")) { sorting = q => q.OrderBy(announ => announ.Price); }
                        else { sorting = q => q.OrderByDescending(announ => announ.Price); }
                        break;

                    case WC.SortByField_Data:
                        if (!isAscending.Equals("")) { sorting = q => q.OrderBy(announ => announ.CreateDt); }
                        else { sorting = q => q.OrderByDescending(announ => announ.CreateDt); }
                        break;
                }
            }

            // фильтрация
            announcements = idCat.Equals("-1") ?
                announcements.Where(announ => announ.Title.ToLower().Contains(titleSearch.ToLower())) :
                announcements.Where(announ => announ.Category.Id.ToString().Equals(idCat));

            // сортировка
            if (sorting is not null)
            {
                IQueryable<Announcement> queryableAnnouncements = announcements.AsQueryable();
                queryableAnnouncements = sorting(queryableAnnouncements);
                announcements = queryableAnnouncements.ToList();
            }

            // пагинация
            Pager pager = new(announcements.Count(), numPage, PageSize);
            int numsSkip = (numPage - 1) * PageSize;
            announcements = announcements.ToList().Skip(numsSkip).Take(PageSize);

            return new AnnouncementVM()
            {
                Announcements = announcements,
                Pager = pager
            };
        }

        public async Task<IActionResult> Index()
        {
            // получаем выбранную категорию и поисковую строку
            string cookieIdCat = _cookieService.GetCookie(WC.CookieCategoryIdKey) ?? "-1";
            string cookieTitleSearch = _cookieService.GetCookie(WC.CookieSearchTitleKey) ?? String.Empty;
            string? cookieSort = _cookieService.GetCookie(WC.CookieSortKey) ?? null;
            string cookieIsAscending = _cookieService.GetCookie(WC.CookieIsAscendingKey) ?? "";

            ViewBag.CookieSort = cookieSort;  // передаём название поля сортировки во view
            ViewBag.CookieIsAscending = cookieIsAscending;  // передаём направление сортировки во view

            return View(await GetAnnouncementVM(cookieIdCat, cookieTitleSearch, 1, cookieSort, cookieIsAscending));
        }

        [HttpPost]
        public async Task<IActionResult> SearchByCategory(string idCat = "-1")
        {
            _cookieService.SetCookie(WC.CookieSearchTitleKey, String.Empty);  // обнуляем данные о поисковой строке
            _cookieService.SetCookie(WC.CookieCategoryIdKey, idCat, DateTimeOffset.UtcNow.AddMinutes(1));  // добавляем id категории
            string? cookieSort = _cookieService.GetCookie(WC.CookieSortKey) ?? null;
            string cookieIsAscending = _cookieService.GetCookie(WC.CookieIsAscendingKey) ?? "";
            return PartialView("_AnnouncementList", await GetAnnouncementVM(idCat, sortBy: cookieSort, isAscending: cookieIsAscending));
        }

        [HttpPost]
        public async Task<IActionResult> SwitchPage(int numPage = 1)
        {
            // получаем выбранную категорию, поисковую строку и сортировку
            if (numPage < 1) { numPage = 1; }
            string cookieIdCat = _cookieService.GetCookie(WC.CookieCategoryIdKey) ?? "-1";
            string cookieTitleSearch = _cookieService.GetCookie(WC.CookieSearchTitleKey) ?? String.Empty;
            string? cookieSort = _cookieService.GetCookie(WC.CookieSortKey) ?? null;
            string cookieIsAscending = _cookieService.GetCookie(WC.CookieIsAscendingKey) ?? "";
            return PartialView("_AnnouncementList", await GetAnnouncementVM(cookieIdCat, cookieTitleSearch, numPage, cookieSort, cookieIsAscending));
        }

        [HttpPost]
        public async Task<IActionResult> SearchByText(string _titleFilter)
        {
            _cookieService.SetCookie(WC.CookieCategoryIdKey, "-1");  // обнуляем данные о выбранной категории
            _cookieService.SetCookie(WC.CookieSearchTitleKey, _titleFilter, DateTimeOffset.UtcNow.AddMinutes(1));
            string? cookieSort = _cookieService.GetCookie(WC.CookieSortKey) ?? null;
            string cookieIsAscending = _cookieService.GetCookie(WC.CookieIsAscendingKey) ?? "";
            return PartialView("_AnnouncementList", await GetAnnouncementVM(titleSearch: _titleFilter, sortBy: cookieSort, isAscending: cookieIsAscending));
        }

        [HttpPost]
        public async Task<IActionResult> Sort(string? sortBy = null, string isAscending = "true")
        {
            // один обработчик для выбора сортировки и выбора направления
            _cookieService.SetCookie(WC.CookieSortKey, sortBy ?? "");
            _cookieService.SetCookie(WC.CookieIsAscendingKey, isAscending ?? "");
            string cookieIdCat = _cookieService.GetCookie(WC.CookieCategoryIdKey) ?? "-1";
            string cookieTitleSearch = _cookieService.GetCookie(WC.CookieSearchTitleKey) ?? String.Empty;
            return PartialView("_AnnouncementList", await GetAnnouncementVM(cookieIdCat, cookieTitleSearch, 1, sortBy, isAscending ?? ""));
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
            try
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string newFilename = await _filesWorkService.DownloadFileForm(WC.ImageAnnouncementPath, files[0]);
                    announcement.Image = newFilename;
                }

                announcement.IdUser = _currentUserProvider.GetCurrentUserId();
                await _announcementRepository.Add(announcement);
                await _announcementRepository.Save();

                await UpdateCacheAllAnnouncement();  // обновляем кэш
            }
            catch (Exception) { }

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


        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if (!Guid.TryParse(id, out Guid announId)) { return NotFound(); }

            var announcement = await _announcementRepository.FirstOrDefault(
                a => Guid.Equals(announId, a.Id),
                includeProps: "Category"
            );
            if (announcement is null) { return NotFound(); }

            ViewData["CategoryItems"] = _announcementRepository.GetAllDropDownList("Category");
            return View(nameof(Create), announcement);
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(Announcement editAnnouncement)
        {
            try
            {
                Announcement? announcement = await _announcementRepository.FirstOrDefault(
                a => a.Id.Equals(editAnnouncement.Id)
            );
                if (announcement is null) { return NotFound(); }

                // обработка изображения
                var files = HttpContext.Request.Form.Files;
                if (files.Count != 0)
                {
                    // загружаем картинку на сервер и сохраняем
                    string newFilename = await _filesWorkService.DownloadFileForm(WC.ImageAnnouncementPath, files[0], announcement.Image);
                    editAnnouncement.Image = newFilename;
                }
                else
                {
                    editAnnouncement.Image = announcement.Image;
                }

                _announcementRepository.Update(announcement, editAnnouncement);
                await _announcementRepository.Save();

                await UpdateCacheAllAnnouncement();  // обновляем кэш
            }
            catch (Exception) { }

            return RedirectToAction(nameof(GetUserAnnouncement));
        }


        [Authorize]
        public async Task Deactivate(string id)
        {
            await ChangeStateAnnouncement(id, false);
        }

        [Authorize]
        public async Task Activate(string id)
        {
            await ChangeStateAnnouncement(id, true);
        }
    }
}
