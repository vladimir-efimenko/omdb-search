var AppSettings = {
    apiBaseUrl: "api/"
}

var app = new Vue({
    el: "#app",
    data: {
        searchTerm: "",
        totalResults: 0,
        currentPage: 0,
        lastPage: 1,
        PageSize: 10,
        errorMessage: null,
        searchResult: null,
        currentMovie: null
    },
    methods: {
        search: function (pageNo) {
            this.cleanData();
            this.currentPage = pageNo;
            console.info("Searching for " + this.searchTerm);
            var self = this;
            $.ajax({
                url: AppSettings.apiBaseUrl + "search",
                data: { title: this.searchTerm, page: pageNo },
                success: function (data) {
                    if (data.error && data.error !== "") {
                        self.showAlert(data.error);
                    } else {
                        console.info(data);
                        self.searchResult = data.search;
                        self.totalResults = data.totalResults;
                        self.lastPage = Math.ceil(data.totalResults / 10);
                    }
                },
                error: function(error) {
                    self.showAlert(error);
                }
        });
        },
        cleanData: function() {
            this.errorMessage = null;
            this.searchResult = null;
            this.currentMovie = null;
        },
        showAlert: function(error) {
            this.errorMessage = error;
            $("alert").show();
        },
        closeAlert: function() {
            $("alert").hide();
            this.errorMessage = null;
        },
        showMovie: function (imdbId) {
            this.currentMovie = null;
            var self = this;
            $.ajax({
                url: AppSettings.apiBaseUrl + "movie/" + imdbId,
                success: function (data) {
                    if (data.error && data.error !== "") {
                        self.showAlert(data.error);
                    } else {
                        console.info(data);
                        self.currentMovie = data;
                        $("#movie-detail").modal("show");
                    }
                },
                error: function (error) {
                    self.showAlert(error);
                }
            });
        }
    }
});