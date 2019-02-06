var AppSettings = {
    apiBaseUrl: "api/"
}

var app = new Vue({
    el: "#app",
    data: {
        searchTerm: "",
        totalResults: 0,
        currentPage: 1,
        PageSize: 10,
        errorMessage: null,
        searchResult: null
    },
    methods: {
        search: function () {
            this.cleanData();
            console.info("Searching for " + this.searchTerm);
            var self = this;
            $.ajax({
                url: AppSettings.apiBaseUrl + "search",
                data: { title: this.searchTerm, page: this.currentPage },
                success: function (data) {
                    if (data.error && data.error !== "") {
                        self.showAlert(data.error);
                    } else {
                        self.searchResult = data.search;
                        self.totalResults = data.totalResults;
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
            console.info("Show movie " + imdbId);
        }
    }
});