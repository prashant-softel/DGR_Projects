function getFinancialYear(today) {
    var fiscalyear = "";
    //var today = new Date();
    if ((today.getMonth() + 1) <= 3) {
        fiscalyear = (today.getFullYear() - 1) + "-" + today.getFullYear().toString().substring(2)
    } else {
        fiscalyear = today.getFullYear() + "-" + (today.getFullYear() + 1).toString().substring(2)
    }
    return fiscalyear;
    
}
function getFinancialYearDate(year) {

    const year_arr = year.split("-");
    let lastyear = parseInt(year_arr[0]) + 1;
    return {
        "startdate": "01-04-" + year_arr[0], "enddate": "31-03-" + lastyear
    };
}
function GetPreviousDate() {
    var today = new Date();
    var dd = String(today.getDate()-1).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
   var yyyy = today.getFullYear();
    //today = mm + '/' + dd + '/' + yyyy;
    return today = yyyy + '-' + mm + '-' + dd;
}
function GetMonthDate(date) {
    // var date = new Date();
    let currentYear = date.getFullYear();
    let currentMonth = date.getMonth()+1;
    let enddate = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    let monthEndDate = enddate.getDate();
    if (currentMonth < 10) {
        currentMonth = '0' + currentMonth;
    }
    return {
        "startdate": `${currentYear}-${currentMonth}-01`, "enddate": `${currentYear}-${currentMonth}-${monthEndDate}`
    };
}
function GetLastTendays() {
    
    var today = new Date();
    var dd = String(today.getDate() - 1).padStart(2, '0');
    var dd1 = String(today.getDate() - 10).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    //today = mm + '/' + dd + '/' + yyyy;
    let enddate = yyyy + '-' + mm + '-' + dd;
    let startDate = yyyy + '-' + mm + '-' + dd1;
    return {
        "startdate": startDate, "enddate": enddate
    };
   // return "abc";
}

