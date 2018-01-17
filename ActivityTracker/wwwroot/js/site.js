﻿function filter() {
    // Declare variables 
    var input, filter, table, tr, td, i;
    input = document.getElementById("search-overview");
    filter = input.value.toUpperCase();
    table = document.getElementById("overview");
    tr = table.getElementsByTagName("tr");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td");
        if (td[0]) {
            if (td[0].innerHTML.toUpperCase().indexOf(filter) > -1 ||
            td[1].innerHTML.toUpperCase().indexOf(filter) > -1 ||
            td[2].innerHTML.toUpperCase().indexOf(filter) > -1
            ) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}

function setTheme(theme) {
    var cssLink = document.getElementById("site-sheet");

    if (theme === "dark") {
        cssLink.href = "/css/site_dark.css";
    }
    else {
        cssLink.href = "/css/site.min.css";
    }
    localStorage.setItem('theme', theme);
}