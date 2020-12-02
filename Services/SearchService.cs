namespace hellodotnetcore.Services
{
    class SearchService
	{
        private readonly SearchLogService _searchLogService;

        public SearchService(SearchLogService searchLogService) {
            _searchLogService = searchLogService;
        }

        public void search(string searchText)
		{
            _searchLogService.createSearchLog(searchText);
		}
	}
}