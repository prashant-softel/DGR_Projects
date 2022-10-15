using DGRAPIs.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Repositories;
using DGRAPIs.Models;

namespace DGRAPIs.BS
{
    public interface IDGRBS
    {
        Task<int> eQry(string qry);
        Task<List<DailyGenSummary>> GetWindDailyGenSummary(string fromDate, string ToDate);
        Task<List<WindDashboardData>> GetWindDashboardData(string startDate, string endDate, string FY, string sites);
        Task<List<WindDashboardData>> GetWindDashboardDataByLastDay(string startDate, string endDate, string FY, string sites, string date);

        Task<List<WindDashboardData>> GetWindDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month);
        Task<List<WindDashboardData>> GetWindDashboardDataByYearly(string startDate, string endDate, string FY, string sites);

        Task<List<SolarDashboardData>> GetSolarDashboardData(string startDate, string endDate, string FY, string sites);

        Task<List<SolarDashboardData>> GetSolarDashboardDataByLastDay(string startDate, string endDate, string FY, string sites, string date);
        Task<List<SolarDashboardData>> GetSolarDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month);
        Task<List<SolarDashboardData>> GetSolarDashboardDataByYearly(string startDate, string endDate, string FY, string sites);

        Task<List<WindSiteMaster>> GetWindSiteMaster();
        Task<List<WindLocationMaster>> GetWindLocationMaster();
        Task<List<SolarSiteMaster>> GetSolarSiteMaster();
        Task<List<SolarLocationMaster>> GetSolarLocationMaster();
        Task<List<SolarLocationMaster>> GetSolarLocationMasterBySite(string site);
        Task<List<WindDailyGenReports>> GetWindDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg);
        Task<List<WindDailyBreakdownReport>> GetWindDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg);
        Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary();
        Task<List<WindDailyGenReports1>> GetWindDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports2>> GetWindDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports1>> GetWindMonthlyYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports2>> GetWindMonthlyYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports1>> GetWindYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports2>> GetWindYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports>> GetWindWtgFromdailyGenSummary(string state, string site);
        Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise(string fy, string fromDate, string todate);
        Task<List<WindPerformanceReports>> GetWindPerformanceReportBySPVWise(string fy, string fromDate, string todate);
        Task<List<SolarDailyBreakdownReport>> GetSolarDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site);
        Task<int> InsertDailyTargetKPI(List<WindDailyTargetKPI> windDailyTargetKPI);
        Task<int> InsertMonthlyTargetKPI(List<WindMonthlyTargetKPI> windMonthlyTargetKPI);
        Task<int> InsertMonthlyUploadingLineLosses(List<WindMonthlyUploadingLineLosses> windMonthlyUploadingLineLosses);
        Task<int> InsertWindDailyLoadShedding(List<WindDailyLoadShedding> windDailyLoadShedding);
        Task<int> InsertWindJMR(List<WindMonthlyJMR> windMonthlyJMR);
        Task<int> InsertDailyJMR(List<WindDailyJMR> windDailyJMR);
        Task<int> InsertSolarDailyTargetKPI(List<SolarDailyTargetKPI> solarDailyTargetKPI);
        Task<int> InsertSolarMonthlyTargetKPI(List<SolarMonthlyTargetKPI> solarMonthlyTargetKPI);
        Task<int> InsertSolarMonthlyUploadingLineLosses(List<SolarMonthlyUploadingLineLosses> solarMonthlyUploadingLineLosses);
        Task<int> InsertSolarJMR(List<SolarMonthlyJMR> solarMonthlyJMR);
        Task<int> InsertSolarDailyLoadShedding(List<SolarDailyLoadShedding> solarDailyLoadShedding);
        Task<int> InsertSolarInvAcDcCapacity(List<SolarInvAcDcCapacity> solarInvAcDcCapacity);
        Task<int> InsertSolarDailyBDloss(List<SolarDailyBDloss> solarDailyBDloss);
        Task<int> InsertSolarUploadingPyranoMeter1Min(List<SolarUploadingPyranoMeter1Min> solarUploadingPyranoMeter1Min);
        Task<int> InsertSolarUploadingPyranoMeter15Min(List<SolarUploadingPyranoMeter15Min> solarUploadingPyranoMeter15Min);
        Task<List<SolarDailyGenReports1>> GetSolarDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        Task<List<SolarDailyGenReports2>> GetSolarDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        Task<List<SolarDailyGenReports1>> GetSolarMonthlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        Task<List<SolarDailyGenReports2>> GetSolarMonthlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        Task<List<SolarDailyGenReports1>> GetSolarYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        Task<List<SolarDailyGenReports2>> GetSolarYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        Task<List<SolarDailyGenReports>> GetSolarInverterFromdailyGenSummary(string state, string site);
        Task<List<SolarPerformanceReports>> GetSolarPerformanceReportBySiteWise(string fy, string fromDate, string todate);
        Task<List<SolarPerformanceReports>> GetSolarPerformanceReportBySPVWise(string fy, string fromDate, string todate);

        Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary(string fromDate, string ToDate);
        Task<List<WindDailyTargetKPI>> GetWindDailyTargetKPI(string fromDate, string todate);
        Task<List<SolarDailyTargetKPI>> GetSolarDailyTargetKPI(string fromDate, string todate);
        Task<List<WindMonthlyTargetKPI>> GetWindMonthlyTargetKPI(string fy, string month);
        Task<List<SolarMonthlyTargetKPI>> GetSolarMonthlyTargetKPI(string fy, string month);
        Task<List<WindMonthlyUploadingLineLosses>> GetWindMonthlyLineLoss(string fy, string month);
        Task<List<SolarMonthlyUploadingLineLosses>> GetSolarMonthlyLineLoss(string fy, string month);
        Task<List<WindMonthlyJMR>> GetWindMonthlyJMR(string fy, string month);
        Task<List<SolarMonthlyJMR>> GetSolarMonthlyJMR(string fy, string month);
        Task<List<SolarInvAcDcCapacity>> GetSolarACDCCapacity(string site);
        Task<List<UserManagement>> GetUserManagement(string userMail, string date);
        Task<List<SolarBDType>> GetSolarBDType();
        Task<List<WindDailyLoadShedding>> GetWindDailyloadShedding(string site, string fromDate, string toDate);
        Task<List<SolarDailyLoadShedding>> GetSolarDailyloadShedding(string site, string fromDate, string toDate);
        Task<List<DailyGenSummary>> GetWindDailyGenSummaryPending(string date, string site);
        Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummaryPending(string date, string site);
        Task<List<WindDailyBreakdownReport>> GetWindDailyBreakdownPending(string date, string site);
        Task<List<SolarFileBreakdown>> GetSolarDailyBreakdownPending(string date, string site);
        Task<int> UpdateWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary);
        Task<int> UpdateWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport);
        Task<int> DeleteWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary);
        Task<int> DeleteWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport);
        Task<int> UpdateSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary);
        Task<int> DeleteSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary);
        Task<int> UpdateSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown);
        Task<int> DeleteSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown);

        Task<bool> InsertSolarUploadingFilegeneration(List<SolarUploadingFilegeneration> solarUploadingFilegeneration);
        Task<bool> InsertSolarUploadingFileBreakDown(List<SolarUploadingFileBreakDown> solarUploadingFileBreakDown);
       
        //Task<List<UserLogin>> GetUserLogin(string username, string password);

    }
    public class DGRBS : IDGRBS
    {
        private readonly DatabaseProvider databaseProvider;
        private MYSQLDBHelper getDB => databaseProvider.SqlInstance();
        public DGRBS(DatabaseProvider dbProvider)
        {
            databaseProvider = dbProvider;
        }
        public async Task<List<DailyGenSummary>> GetWindDailyGenSummary(string fromDate, string ToDate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyGenSummary(fromDate, ToDate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindDashboardData>> GetWindDashboardData(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDashboardData(startDate, endDate, FY, sites);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDashboardData>> GetWindDashboardDataByLastDay(string startDate, string endDate, string FY, string sites, string date)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDashboardDataByLastDay(startDate, endDate, FY, sites, date);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<WindDashboardData>> GetWindDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDashboardDataByCurrentMonth(startDate, endDate, FY, sites, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDashboardData>> GetWindDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDashboardDataByYearly(startDate, endDate, FY, sites);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SolarDashboardData>> GetSolarDashboardData(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDashboardData(startDate, endDate, FY, sites);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDashboardData>> GetSolarDashboardDataByLastDay(string startDate, string endDate, string FY, string sites, string date)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDashboardDataByLastDay(startDate, endDate, FY, sites, date);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDashboardData>> GetSolarDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDashboardDataByCurrentMonth(startDate, endDate, FY, sites, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDashboardData>> GetSolarDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDashboardDataByYearly(startDate, endDate, FY, sites);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindSiteMaster>> GetWindSiteMaster()
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindSiteMaster();

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindLocationMaster>> GetWindLocationMaster()
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindLocationMaster();

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarSiteMaster>> GetSolarSiteMaster()
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarSiteMaster();

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarLocationMaster>> GetSolarLocationMaster()
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarLocationMaster();

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarLocationMaster>> GetSolarLocationMasterBySite(string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarLocationMasterBySite(site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDailyGenReports>> GetWindDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyGenerationReport(fromDate, toDate, country, state, spv, site, wtg);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDailyBreakdownReport>> GetWindDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyBreakdownReport(fromDate, toDate, country, state, spv, site, wtg);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary()
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyGenSummary();

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindDailyGenReports1>> GetWindDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyGenSummaryReport1(fromDate, toDate, country, state, spv, site, wtg, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDailyGenReports2>> GetWindDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyGenSummaryReport2(fromDate, toDate, country, state, spv, site, wtg, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDailyGenReports1>> GetWindMonthlyYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyYearlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, wtg, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindDailyGenReports2>> GetWindMonthlyYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyYearlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, wtg, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDailyGenReports1>> GetWindYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindYearlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, wtg, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindDailyGenReports2>> GetWindYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindYearlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, wtg, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindDailyGenReports>> GetWindWtgFromdailyGenSummary(string state, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindWtgFromdailyGenSummary(state, site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise(string fy, string fromDate, string todate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindPerformanceReportSiteWise(fy, fromDate, todate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindPerformanceReports>> GetWindPerformanceReportBySPVWise(string fy, string fromDate, string todate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindPerformanceReportBySPVWise(fy, fromDate, todate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyBreakdownReport>> GetSolarDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyBreakdownReport(fromDate, toDate, country, state, spv, site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertDailyTargetKPI(List<WindDailyTargetKPI> windDailyTargetKPI)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertDailyTargetKPI(windDailyTargetKPI);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertMonthlyTargetKPI(List<WindMonthlyTargetKPI> windMonthlyTargetKPI)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertMonthlyTargetKPI(windMonthlyTargetKPI);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<int> InsertMonthlyUploadingLineLosses(List<WindMonthlyUploadingLineLosses> windMonthlyUploadingLineLosses)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertMonthlyUploadingLineLosses(windMonthlyUploadingLineLosses);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> InsertWindJMR(List<WindMonthlyJMR> windMonthlyJMR)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertWindJMR(windMonthlyJMR);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertWindDailyLoadShedding(List<WindDailyLoadShedding> windDailyLoadShedding)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertWindDailyLoadShedding(windDailyLoadShedding);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> InsertDailyJMR(List<WindDailyJMR> windDailyJMR)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertDailyJMR(windDailyJMR);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<int> InsertSolarDailyTargetKPI(List<SolarDailyTargetKPI> solarDailyTargetKPI)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarDailyTargetKPI(solarDailyTargetKPI);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarMonthlyTargetKPI(List<SolarMonthlyTargetKPI> solarMonthlyTargetKPI)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarMonthlyTargetKPI(solarMonthlyTargetKPI);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> InsertSolarMonthlyUploadingLineLosses(List<SolarMonthlyUploadingLineLosses> solarMonthlyUploadingLineLosses)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarMonthlyUploadingLineLosses(solarMonthlyUploadingLineLosses);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarJMR(List<SolarMonthlyJMR> solarMonthlyJMR)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarJMR(solarMonthlyJMR);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarDailyLoadShedding(List<SolarDailyLoadShedding> solarDailyLoadShedding)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarDailyLoadShedding(solarDailyLoadShedding);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarInvAcDcCapacity(List<SolarInvAcDcCapacity> solarInvAcDcCapacity)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarInvAcDcCapacity(solarInvAcDcCapacity);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarDailyBDloss(List<SolarDailyBDloss> solarDailyBDloss)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarDailyBDloss(solarDailyBDloss);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> InsertSolarUploadingPyranoMeter1Min(List<SolarUploadingPyranoMeter1Min> solarUploadingPyranoMeter1Min)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarUploadingPyranoMeter1Min(solarUploadingPyranoMeter1Min);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarUploadingPyranoMeter15Min(List<SolarUploadingPyranoMeter15Min> solarUploadingPyranoMeter15Min)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarUploadingPyranoMeter15Min(solarUploadingPyranoMeter15Min);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertSolarUploadingFilegeneration(List<SolarUploadingFilegeneration> solarUploadingFilegeneration)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarUploadingFilegeneration(solarUploadingFilegeneration);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertSolarUploadingFileBreakDown(List<SolarUploadingFileBreakDown> solarUploadingFileBreakDown)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarUploadingFileBreakDown(solarUploadingFileBreakDown);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SolarDailyGenReports1>> GetSolarDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyGenSummaryReport1(fromDate, toDate, country, state, spv, site, inverter, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyGenReports2>> GetSolarDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inverter, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SolarDailyGenReports1>> GetSolarMonthlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, inverter, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyGenReports2>> GetSolarMonthlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inverter, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SolarDailyGenReports1>> GetSolarYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarYearlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, inverter, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyGenReports2>> GetSolarYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarYearlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inverter, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<SolarDailyGenReports>> GetSolarInverterFromdailyGenSummary(string state, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarInverterFromdailyGenSummary(state,site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarPerformanceReports>> GetSolarPerformanceReportBySiteWise(string fy, string fromDate, string todate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarPerformanceReportBySiteWise(fy, fromDate, todate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarPerformanceReports>> GetSolarPerformanceReportBySPVWise(string fy, string fromDate, string todate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarPerformanceReportBySPVWise(fy, fromDate, todate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
      
        public async Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary(string fromDate, string ToDate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyGenSummary(fromDate, ToDate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindDailyTargetKPI>> GetWindDailyTargetKPI(string fromDate, string todate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyTargetKPI(fromDate, todate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarDailyTargetKPI>> GetSolarDailyTargetKPI(string fromDate, string todate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyTargetKPI(fromDate, todate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindMonthlyTargetKPI>> GetWindMonthlyTargetKPI(string fy, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyTargetKPI(fy, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarMonthlyTargetKPI>> GetSolarMonthlyTargetKPI(string fy, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyTargetKPI(fy, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindMonthlyUploadingLineLosses>> GetWindMonthlyLineLoss(string fy, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyLineLoss(fy, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarMonthlyUploadingLineLosses>> GetSolarMonthlyLineLoss(string fy, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyLineLoss(fy, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindMonthlyJMR>> GetWindMonthlyJMR(string fy, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyJMR(fy, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarMonthlyJMR>> GetSolarMonthlyJMR(string fy, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyJMR(fy, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarInvAcDcCapacity>> GetSolarACDCCapacity(string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarACDCCapacity(site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<UserManagement>> GetUserManagement(string userMail, string date)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetUserManagement(userMail,date);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarBDType>> GetSolarBDType()
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarBDType();

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<WindDailyLoadShedding>> GetWindDailyloadShedding(string site, string fromDate, string toDate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyloadShedding(site,fromDate,toDate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyLoadShedding>> GetSolarDailyloadShedding(string site, string fromDate, string toDate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyloadShedding(site,fromDate,toDate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<DailyGenSummary>> GetWindDailyGenSummaryPending(string date, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyGenSummaryPending(date,site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummaryPending(string date,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyGenSummaryPending(date, site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDailyBreakdownReport>> GetWindDailyBreakdownPending(string date, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyBreakdownPending(date,site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarFileBreakdown>> GetSolarDailyBreakdownPending(string date,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyBreakdownPending(date, site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> UpdateWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.UpdateWindDailyGenSummaryApproveStatus(dailyGenSummary);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> UpdateWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.UpdateWindDailyBreakdownApproveStatus(windDailyBreakdownReport);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> DeleteWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.DeleteWindDailyGenSummaryApproveStatus(dailyGenSummary);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> DeleteWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.DeleteWindDailyBreakdownApproveStatus(windDailyBreakdownReport);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> UpdateSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.UpdateSolarDailyGenSummaryApproveStatus(solarDailyGenSummary);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> DeleteSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.DeleteSolarDailyGenSummaryApproveStatus(solarDailyGenSummary);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> UpdateSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.UpdateSolarDailyBreakdownApproveStatus(solarFileBreakdown);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> DeleteSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.DeleteSolarDailyBreakdownApproveStatus(solarFileBreakdown);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
       /* public async Task<List<UserLogin>> GetUserLogin(string username, string password)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetLogin(username, password);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }*/
        public async Task<int> eQry(string qry)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.eQry(qry);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
