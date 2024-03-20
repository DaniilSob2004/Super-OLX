namespace OnlineClassifieds.Helpers
{
    public class Pager
    {
        public int TotalItems { get; private set; }  // кол-во элементов
        public int CurrentPage { get; private set; }  // номер текущей страницы
        public int PageSize { get; private set; }  // кол-во элементов на странице
        public int TotalPages { get; private set; }  // кол-во страниц
        public int StartPage { get; private set; }  // номер начальной страницы
        public int EndPage { get; private set; }  // номер последней страницы

        public Pager(int totalItems, int currentPage, int pageSize = 3)
        {
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);  // кол-во страниц (округляем в большую сторону)

            // определение диапазона страниц, которые будут отображаться в пагинации
            int startPage = currentPage - 5;  // начальная страница в диапазоне (на 5 страниц назад от текущей страницы)
            int endPage = currentPage + 4;

            if (startPage <= 0) { startPage = 1; }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)  // если страница больше 10, то начальную делаем -9, чтобы отображалось 10 страниц
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
