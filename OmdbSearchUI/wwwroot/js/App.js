let AppSettings = {
    apiBaseUrl: "api/"
}

let app = new Vue({
    el: "#app",
    data: {
        searchTerm: "",
        currentPage: 1,
        totalPages: 0,
        errorMessage: null
    },
    methods: {
        search: function() {
            console.info("Searching for " + this.searchTerm);
            let self = this;
            $.ajax({
                url: AppSettings.apiBaseUrl + "search",
                data: { title: this.searchTerm, page: this.currentPage },
                success: function (data) {
                    if (data.error && data.error !== "") {
                        self.errorMessage = data.error;
                        $("alert").show();
                    }

                }
        });
        },
        closeAlert: function() {
            $("alert").hide();
            this.errorMessage = null;
        }
    }
});