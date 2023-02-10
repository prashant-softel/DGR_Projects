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
function getFinancialYearDateStartDate(year) {

    const year_arr = year.split("-");
    let lastyear = parseInt(year_arr[0]) + 1;
    return {
        "startdate": year_arr[0] + "-04-01", "enddate": lastyear + "-03-31"
    };
}
function GetPreviousDate() {
    var today = new Date();
    today.setDate(today.getDate() - 1);
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    //today = mm + '/' + dd + '/' + yyyy;
    return today = yyyy + '-' + mm + '-' + dd;
}
function GetMonthDate(date) {
    // var date = new Date();
    //let todayDate = new Date();
    if (date.getDate() > 1) {
        date.setDate(date.getDate() - 1);
    }

    let startDate = new Date(date.getFullYear(), date.getMonth(), 1);

    //moment(startDate).format('YYYY-MM-DD')
    //let currentYear = date.getFullYear();
    //let currentMonth = date.getMonth()+1;
    //let enddate = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    //let currentDate = date.getDate();
    //if (currentDate > 1) {
    //    currentDate - 1;
    //}
    //console.log("ENd Month :" + currentDate);
    
    return {
        "startdate": `${moment(startDate).format('YYYY-MM-DD')}`, "enddate": `${moment(date).format('YYYY-MM-DD')}`
    };
    return {
        "startdate": `${currentYear}-${currentMonth}-01`, "enddate": `${currentYear}-${currentMonth}-${currentDate}`
    };
}
function GetLastTendays() {
    
    var today = new Date();
    var today1 = new Date();
    today.setDate(today.getDate() - 10);
    today1.setDate(today1.getDate() - 1);
    var dd = String(today.getDate()).padStart(2, '0');
    var dd1 = String(today.getDate()).padStart(2, '0');
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

function GetWeeklyDays(seldate) {
    //console.log("Selected Date :"+seldate);
    seldate.setDate(seldate.getDate() - 6);
    var dd = String(seldate.getDate() ).padStart(2, '0');
    var mm = String(seldate.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = seldate.getFullYear();
   // console.log("DD" + dd);
    //console.log("MM" + mm);
    //console.log("yy" + yyyy);
    //today = mm + '/' + dd + '/' + yyyy;
   let today = yyyy + '-' + mm + '-' + dd;
    //console.log(today);
    return today;
    // return "abc";
}
function toHoursAndMinutes(totalSeconds) {
   // console.log("second", totalSeconds);
    const totalMinutes = Math.floor(totalSeconds / 60);

    const s = totalSeconds % 60;
    const h = Math.floor(totalMinutes / 60);
    const m = totalMinutes % 60;
    
    const hDisplay = h > 0 ? `${h.toString().length > 1 ? `${h}` : `${0}${h}`}` : '00';
    const mDisplay = m > 0 ? `${m.toString().length > 1 ? `${m}` : `${0}${m}`}` : '00';
    const sDisplay = s > 0 ? `${s.toString().length > 1 ? `${s}` : `${0}${s}`}` : '00';
   // console.log(`${hDisplay}:${mDisplay}:${sDisplay}`);
    return `${hDisplay}:${mDisplay}:${sDisplay}`;
    
   // return { h: hours, m: minutes, s: seconds };
}

