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
        Task<int> GetBatchStatus(int import_type, int site_id, string import_date);
        //Task<List<ImportBatchStatus>> GetBatchStatus(int import_type, int site_id, string import_date);
        Task<BatchIdImport> GetBatchId(string logFileName);
        Task<int> DeleteRecordsAfterFailure(int batchId, int siteType);
        Task<List<FinancialYear>> GetFinancialYear();
        Task<List<DailyGenSummary>> GetWindDailyGenSummary(string site, string fromDate, string ToDate);
        Task<List<WindDashboardData>> GetWindDashboardData(string startDate, string endDate, string FY, string sites,bool monthly);
        Task<List<WindDashboardData>> GetWindDashboardDataByLastDay(string FY, string sites, string date);

        Task<List<WindDashboardData>> GetWindDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month);
        Task<List<WindDashboardData>> GetWindDashboardDataByYearly(string startDate, string endDate, string FY, string sites);

        Task<List<SolarDashboardData>> GetSolarDashboardData(string startDate, string endDate, string FY, string sites,bool monthly);

        Task<List<SolarDashboardData>> GetSolarDashboardDataByLastDay(string FY, string sites, string date);
        Task<List<SolarDashboardData>> GetSolarDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month);
        Task<List<SolarDashboardData>> GetSolarDashboardDataByYearly(string startDate, string endDate, string FY, string sites);

        Task<List<WindSiteMaster>> GetWindSiteMaster(string site);
        Task<List<WindLocationMaster>> GetWindLocationMaster(string site);
        Task<List<SolarSiteMaster>> GetSolarSiteMaster(string site);
        Task<List<SolarLocationMaster>> GetSolarLocationMaster();
        Task<List<SolarLocationMaster>> GetSolarLocationMasterBySite(string site);
        Task<List<WindDailyGenReports>> GetWindDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg,string reportType);
        Task<List<SolarDailyGenReports>> GetSolarDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string inv, string reportType);
        Task<List<WindDailyBreakdownReport>> GetWindDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg);
        Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary();
        Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary1(string site, string fromDate, string ToDate);
        Task<List<SolarUploadingPyranoMeter1Min_1>> SolarGhi_Poa_1Min(string site, string fromDate, string ToDate);
        Task<List<SolarUploadingPyranoMeter15Min_1>> SolarGhi_Poa_15Min(string site, string fromDate, string ToDate);
        
        Task<List<WindDailyGenReports1>> GetWindDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports2>> GetWindDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports1>> GetWindMonthlyGenerationReport(string fy, string month, string country, string state, string spv, string site, string wtg, string reportType);
        Task<List<WindDailyGenReports2>> GetWindMonthlyYearlyGenSummaryReport2(string fromDate, string month, string country, string state, string spv, string site, string wtg);
        Task<List<WindDailyGenReports1>> GetWindYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports2>> GetWindYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month);
        Task<List<WindDailyGenReports>> GetWindWtgFromdailyGenSummary(string state, string site);
        Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise_2(string fromDate, string toDate, string site);
        Task<List<SolarPerformanceReports2>> GetSolarPerformanceReportSiteWise_2(string fromDate, string toDate, string site,int cnt);
        Task<List<SolarUploadingFileBreakDown>> GetSolarMajorBreakdownData(string fromDate, string toDate, string site);
        //Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise_3(string fromDate, string toDate, string site);
        //Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise_4(string fromDate, string toDate, string site);
        Task <string> EmailWindReport(string fy, string fromDate,  string site);
        Task<string> EmailSolarReport(string fy, string fromDate, string site);
        Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise(string fy, string fromDate, string todate,string site);
        Task<List<WindPerformanceReports>> GetWindPerformanceReportBySPVWise(string fy, string fromDate, string todate,string site);
        Task<List<SolarDailyBreakdownReport>> GetSolarDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string inv);
        Task<bool> CalculateDailyWindKPI(string fromDate, string toDate, string site);
        Task<bool> CalculateDailySolarKPI(string site, string fromDate, string toDate, string logFileName);
        Task<int> InsertDailyTargetKPI(List<WindDailyTargetKPI> set);
        Task<int> InsertMonthlyTargetKPI(List<WindMonthlyTargetKPI> set);
        Task<int> InsertMonthlyUploadingLineLosses(List<WindMonthlyUploadingLineLosses> set);
        Task<int> InsertWindDailyLoadShedding(List<WindDailyLoadShedding> set);
        Task<int> InsertWindJMR(List<WindMonthlyJMR> set);
        Task<int> InsertWindUploadingFileGeneration(List<WindUploadingFileGeneration> set, int batchId);
        Task<int> InsertWindUploadingFileBreakDown(List<WindUploadingFileBreakDown> set, int batchId);
        Task<List<SolarSiteMaster>> GetSolarSiteList(string state, string spvdata, string site);
        //Task<int> InsertDailyJMR(List<WindDailyJMR> set);
        Task<int> InsertSolarDailyTargetKPI(List<SolarDailyTargetKPI> set);
        Task<int> InsertSolarMonthlyTargetKPI(List<SolarMonthlyTargetKPI> set);
        Task<int> InsertSolarMonthlyUploadingLineLosses(List<SolarMonthlyUploadingLineLosses> set);
        Task<int> InsertSolarJMR(List<SolarMonthlyJMR> set);
        Task<int> InsertSolarDailyLoadShedding(List<SolarDailyLoadShedding> set);
        Task<int> InsertSolarInvAcDcCapacity(List<SolarInvAcDcCapacity> set);
        Task<int> InsertSolarDailyBDloss(List<SolarDailyBDloss> set);
        Task<int> InsertSolarUploadingPyranoMeter1Min(List<SolarUploadingPyranoMeter1Min> set, int batchId);
        Task<int> InsertSolarUploadingPyranoMeter15Min(List<SolarUploadingPyranoMeter15Min> set, int batchId);
        Task<List<SolarDailyGenReports1>> GetSolarDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        //Task<List<SolarDailyGenReports2>> GetSolarDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        Task<List<SolarDailyGenReports2>> GetSolarDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inv, string month);
        Task<List<SolarDailyGenReports1>> GetSolarMonthlyGenSummaryReport1(string fy, string month, string country, string state, string spv, string site, string inverter);
        Task<List<SolarDailyGenReports2>> GetSolarMonthlyGenSummaryReport2(string fy, string month, string country, string state, string spv, string site, string inverter);
        Task<List<SolarDailyGenReports1>> GetSolarYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        Task<List<SolarDailyGenReports2>> GetSolarYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month);
        Task<List<SolarDailyGenReports>> GetSolarInverterFromdailyGenSummary(string state, string site);
        Task<List<SolarPerformanceReports1>> GetSolarPerformanceReportBySiteWise(string fy, string fromDate, string todate,string site);
        Task<List<SolarPerformanceReports1>> GetSolarPerformanceReportBySPVWise(string fy, string fromDate, string todate, string site);

        Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary(string fromDate, string ToDate);
        Task<List<WindDailyTargetKPI>> GetWindDailyTargetKPI(string  site, string fromDate, string todate);
        Task<List<SolarDailyTargetKPI>> GetSolarDailyTargetKPI(string fromDate, string todate, string site);
        Task<List<WindMonthlyTargetKPI>> GetWindMonthlyTargetKPI(string site, string fy, string month);
        Task<List<SolarMonthlyTargetKPI>> GetSolarMonthlyTargetKPI(string fy, string month, string site);
        Task<List<WindMonthlyUploadingLineLosses>> GetWindMonthlyLineLoss(string site, string fy, string month);
        Task<List<SolarMonthlyUploadingLineLosses>> GetSolarMonthlyLineLoss(string fy, string month, string site);
        Task<List<WindMonthlyJMR1>> GetWindMonthlyJMR(string site, string fy, string month);
        Task<List<SolarMonthlyJMR>> GetSolarMonthlyJMR(string fy, string month, string site);
        Task<List<SolarInvAcDcCapacity>> GetSolarACDCCapacity(string site);
        Task<List<UserManagement>> GetUserManagement(string userMail, string date);
        //Task<List<SolarBDType>> GetSolarBDType();
        Task<List<WindViewDailyLoadShedding>> GetWindDailyloadShedding(string site, string fromDate, string toDate);
        Task<List<SolarDailyLoadShedding>> GetSolarDailyloadShedding(string site, string fromDate, string toDate);
        Task<List<DailyGenSummary>> GetWindDailyGenSummaryPending(string date, string site);
        Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummaryPending(string date, string site);
        Task<List<WindDailyBreakdownReport>> GetWindDailyBreakdownPending(string date, string site);
        Task<List<SolarFileBreakdown>> GetSolarDailyBreakdownPending(string date, string site);
        Task<int> InsertWindSiteMaster(List<WindSiteMaster> set);
        Task<int> InsertWindLocationMaster(List<WindLocationMaster> set);
        
        Task<int> InsertSolarLocationMaster(List<SolarLocationMaster> set);
        Task<int> InsertSolarSiteMaster(List<SolarSiteMaster> set);

        Task<int> PPTCreate(string fy, string startDate, string endDate, string type);
        Task<int> MailSend(string fname);


        Task<int> UpdateWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary);
        Task<int> UpdateWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport);
        Task<int> DeleteWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary);
        Task<int> DeleteWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport);
        Task<int> UpdateSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary);
        Task<int> DeleteSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary);
        Task<int> UpdateSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown);
        Task<int> DeleteSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown);

        Task<int> InsertSolarUploadingFileGeneration(List<SolarUploadingFileGeneration> set, int batchId);
        Task<int> InsertSolarUploadingFileBreakDown(List<SolarUploadingFileBreakDown> set, int batchId);

        Task<List<approvalObject>> GetImportBatches(string importFromDate, string importToDate, string siteId, int importType, int status,int userid);
        Task<int> SetApprovalFlagForImportBatches(string dataId, int approvedBy, string approvedByName, int status);
        Task<int> SetRejectFlagForImportBatches(string dataId, int rejectedBy, string rejectByName, int status);
        Task<int> SetSolarApprovalFlagForImportBatches(string dataId, int approvedBy, string approvedByName, int status);
        Task<int> SetSolarRejectFlagForImportBatches(string dataId, int rejectedBy, string rejectByName, int status);
        Task<List<CountryList>> GetCountryList();
        Task<List<StateList>> GetStateList(string country,string site);
        Task<List<StateList>> GetStateListSolar(string country, string site);
        Task<List<SPVList>> GetSPVList(string state,string site);
        Task<List<SPVList>> GetSPVListSolar(string state,string site);
        Task<List<WindSiteMaster>> GetSiteList(string state, string spvdata,string site);
        Task<List<SolarSiteMaster>> GetSiteListSolar(string state, string spvdata, string site);
        Task<List<WindLocationMaster>> GetWTGList(string siteid);
        Task<List<SolarLocationMaster>> GetInvList(string siteid, string state, string spv);
        Task<List<BDType>> GetBDType();
        Task<List<WindUploadingFilegeneration1>> GetImportGenData(int importId);
        Task<List<WindUploadingFileBreakDown1>> GetBrekdownImportData(int importId);
        Task<List<SolarUploadingFileGeneration2>> GetSolarImportGenData(int importId);
        Task<List<SolarUploadingFileBreakDown1>> GetSolarBrekdownImportData(int importId);
        Task<List<SolarUploadingPyranoMeter1Min_1>> GetSolarP1ImportData(int importId);
        Task<List<SolarUploadingPyranoMeter15Min_1>> GetSolarP15ImportData(int importId);

        Task<int> importMetaData(ImportBatch meta,string userName,int userId);
        Task<List<WindOpertionalHead>> GetOperationHeadData(string site);
		Task<List<SolarOpertionalHead>> GetSolarOperationHeadData(string site);
        Task<List<WindUploadingFileBreakDown>> GetWindMajorBreakdown(string fromDate, string toDate,string site);
        Task<int> DeleteWindSite(int siteid);
        Task<int> DeleteSolarSite(int siteid);
        Task<List<SolarOpertionalHead1>> GetTotalMWforDashbord(string w_site, string s_site);
        //Task<WindOpertionalHead> GetOperationHeadData(string site);
    }
    public class DGRBS : IDGRBS
    {
        private readonly DatabaseProvider databaseProvider;
        private MYSQLDBHelper getDB => databaseProvider.SqlInstance();
        public DGRBS(DatabaseProvider dbProvider)
        {
            databaseProvider = dbProvider;
        }
        public async Task<List<FinancialYear>> GetFinancialYear()
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetFinancialYear();

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<DailyGenSummary>> GetWindDailyGenSummary(string site, string fromDate, string ToDate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyGenSummary(site, fromDate, ToDate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindDashboardData>> GetWindDashboardData(string startDate, string endDate, string FY, string sites,bool monthly)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDashboardData(startDate, endDate, FY, sites, monthly);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDashboardData>> GetWindDashboardDataByLastDay(string FY, string sites, string date)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDashboardDataByLastDay(FY, sites, date);

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

        public async Task<List<SolarDashboardData>> GetSolarDashboardData(string startDate, string endDate, string FY, string sites,bool monthly)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDashboardData(startDate, endDate, FY, sites,monthly);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDashboardData>> GetSolarDashboardDataByLastDay(string FY, string sites, string date)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDashboardDataByLastDay(FY, sites, date);

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

        public async Task<List<WindSiteMaster>> GetWindSiteMaster(string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindSiteMaster(site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindLocationMaster>> GetWindLocationMaster(string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindLocationMaster(site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarSiteMaster>> GetSolarSiteMaster(string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarSiteMaster(site);

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

        public async Task<List<WindDailyGenReports>> GetWindDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string reportType)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyGenerationReport(fromDate, toDate, country, state, spv, site, wtg, reportType);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyGenReports>> GetSolarDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string inv, string reportType)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyGenerationReport(fromDate, toDate, country, state, spv, site, inv, reportType);

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
        public async Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary1(string site, string fromDate, string ToDate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyGenSummary1(site, fromDate,  ToDate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarUploadingPyranoMeter1Min_1>> SolarGhi_Poa_1Min(string site, string fromDate, string ToDate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.SolarGhi_Poa_1Min(site, fromDate, ToDate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarUploadingPyranoMeter15Min_1>> SolarGhi_Poa_15Min(string site, string fromDate, string ToDate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.SolarGhi_Poa_15Min(site, fromDate, ToDate);

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
        public async Task<List<SolarDailyGenReports2>> GetSolarDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inv, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inv, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<WindDailyGenReports1>> GetWindMonthlyGenerationReport(string fy, string month, string country, string state, string spv, string site, string wtg, string reportType)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyGenerationReport(fy, month, country, state, spv, site, wtg, reportType);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindDailyGenReports2>> GetWindMonthlyYearlyGenSummaryReport2(string fy, string month, string country, string state, string spv, string site, string wtg)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyYearlyGenSummaryReport2(fy, month, country, state, spv, site, wtg);

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
        public async Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise_2(string fromDate, string toDate, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindPerformanceReportSiteWise_2(fromDate, toDate, site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarPerformanceReports2>> GetSolarPerformanceReportSiteWise_2(string fromDate, string toDate, string site,int cnt)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarPerformanceReportSiteWise_2(fromDate, toDate, site, cnt);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarUploadingFileBreakDown>> GetSolarMajorBreakdownData(string fromDate, string toDate, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMajorBreakdownData(fromDate, toDate, site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
       /* public async Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise_3(string fromDate, string toDate, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindPerformanceReportSiteWise_3(fromDate, toDate, site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise_4(string fromDate, string toDate, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindPerformanceReportSiteWise_4(fromDate, toDate, site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }*/
        public async Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise(string fy, string fromDate, string todate,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindPerformanceReportSiteWise(fy, fromDate, todate,site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<WindPerformanceReports>> GetWindPerformanceReportBySPVWise(string fy, string fromDate, string todate,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindPerformanceReportBySPVWise(fy, fromDate, todate,site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyBreakdownReport>> GetSolarDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string inv)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyBreakdownReport(fromDate, toDate, country, state, spv, site, inv);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CalculateDailyWindKPI(string fromDate, string toDate, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.CalculateDailyWindKPI(fromDate, toDate, site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CalculateDailySolarKPI(string site, string fromDate, string toDate, string logFileName)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.CalculateDailySolarKPI(site, fromDate, toDate, logFileName);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<int> InsertWindSiteMaster(List<WindSiteMaster> set)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertWindSiteMaster(set);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertWindLocationMaster(List<WindLocationMaster> set)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertWindLocationMaster(set);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarLocationMaster(List<SolarLocationMaster> set)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarLocationMaster(set);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarSiteMaster(List<SolarSiteMaster> set)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarSiteMaster(set);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> MailSend(string fname)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.MailSend(fname);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> PPTCreate(string fy, string startDate, string endDate, string type)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.PPTCreate(fy, startDate, endDate, type);

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
        public async Task<int> InsertMonthlyTargetKPI(List<WindMonthlyTargetKPI> set)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertMonthlyTargetKPI(set);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<int> InsertMonthlyUploadingLineLosses(List<WindMonthlyUploadingLineLosses> set)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertMonthlyUploadingLineLosses(set);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> InsertWindJMR(List<WindMonthlyJMR> set)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertWindJMR(set);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertWindDailyLoadShedding(List<WindDailyLoadShedding> set)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertWindDailyLoadShedding(set);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarSiteMaster>> GetSolarSiteList(string state, string spvdata, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarSiteData(state, spvdata, site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
       /* public async Task<int> InsertDailyJMR(List<WindDailyJMR> windDailyJMR)
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

        }*/

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
        public async Task<int> InsertSolarJMR(List<SolarMonthlyJMR> set)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarJMR(set);

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

        public async Task<int> InsertSolarUploadingPyranoMeter1Min(List<SolarUploadingPyranoMeter1Min> set, int batchId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarUploadingPyranoMeter1Min(set, batchId);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarUploadingPyranoMeter15Min(List<SolarUploadingPyranoMeter15Min> set, int batchId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarUploadingPyranoMeter15Min(set, batchId);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> importMetaData(ImportBatch meta, string userName, int userId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.importMetaData(meta, userName, userId);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertSolarUploadingFileGeneration(List<SolarUploadingFileGeneration> set, int batchId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarUploadingFileGeneration(set, batchId);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> InsertSolarUploadingFileBreakDown(List<SolarUploadingFileBreakDown> set, int batchId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertSolarUploadingFileBreakDown(set, batchId);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> InsertWindUploadingFileGeneration(List<WindUploadingFileGeneration> set, int batchId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertWindUploadingFileGeneration(set, batchId);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> InsertWindUploadingFileBreakDown(List<WindUploadingFileBreakDown> set, int batchId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.InsertWindUploadingFileBreakDown(set, batchId);
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
        //public async Task<List<SolarDailyGenReports2>> GetSolarDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        //{
        //    try
        //    {
        //        using (var repos = new DGRRepository(getDB))
        //        {
        //            return await repos.GetSolarDailyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inverter, month);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public async Task<List<SolarDailyGenReports1>> GetSolarMonthlyGenSummaryReport1(string fy, string month, string country, string state, string spv, string site, string inverter)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyGenSummaryReport1(fy, month, country, state, spv, site, inverter);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarDailyGenReports2>> GetSolarMonthlyGenSummaryReport2(string fy, string month, string country, string state, string spv, string site, string inverter)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyGenSummaryReport2(fy, month, country, state, spv, site, inverter);

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
        public async Task<List<SolarPerformanceReports1>> GetSolarPerformanceReportBySiteWise(string fy, string fromDate, string todate,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarPerformanceReportBySiteWise(fy, fromDate, todate,site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<SolarPerformanceReports1>> GetSolarPerformanceReportBySPVWise(string fy, string fromDate, string todate,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarPerformanceReportBySPVWise(fy, fromDate, todate,site);

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
        public async Task<List<WindDailyTargetKPI>> GetWindDailyTargetKPI(string site, string fromDate, string todate)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindDailyTargetKPI(site, fromDate, todate);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarDailyTargetKPI>> GetSolarDailyTargetKPI(string fromDate, string todate, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarDailyTargetKPI(fromDate, todate, site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindMonthlyTargetKPI>> GetWindMonthlyTargetKPI(string site, string fy, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyTargetKPI(site, fy, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarMonthlyTargetKPI>> GetSolarMonthlyTargetKPI(string fy, string month, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyTargetKPI(fy, month, site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindMonthlyUploadingLineLosses>> GetWindMonthlyLineLoss(string site, string fy, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyLineLoss(site, fy, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarMonthlyUploadingLineLosses>> GetSolarMonthlyLineLoss(string fy, string month, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyLineLoss(fy, month, site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindMonthlyJMR1>> GetWindMonthlyJMR(string site, string fy, string month)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMonthlyJMR(site, fy, month);

                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarMonthlyJMR>> GetSolarMonthlyJMR(string fy, string month, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarMonthlyJMR(fy, month, site);

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
        public async Task<List<BDType>> GetBDType()
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetBDType();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<WindViewDailyLoadShedding>> GetWindDailyloadShedding(string site, string fromDate, string toDate)
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

        public async Task<List<approvalObject>> GetImportBatches(string importFromDate, string importToDate, string siteId,int importType, int status,int userid)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetImportBatches(importFromDate, importToDate, siteId, importType, status, userid);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
       // public async Task<List<approvalObject>> SetApprovalFlagForImportBatches(string dataId, int approvedBy, string approvedByName, int status)
        public async Task<int> SetApprovalFlagForImportBatches(string dataId, int approvedBy, string approvedByName, int status)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.SetApprovalFlagForImportBatches(dataId, approvedBy, approvedByName, status);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<int> SetRejectFlagForImportBatches(string dataId, int rejectedBy, string rejectByName, int status)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.SetRejectFlagForImportBatches(dataId, rejectedBy, rejectByName, status);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<int> SetSolarApprovalFlagForImportBatches(string dataId, int approvedBy, string approvedByName, int status)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.SetSolarApprovalFlagForImportBatches(dataId, approvedBy, approvedByName, status);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<int> SetSolarRejectFlagForImportBatches(string dataId, int rejectedBy, string rejectByName, int status)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.SetSolarRejectFlagForImportBatches(dataId, rejectedBy, rejectByName, status);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<CountryList>> GetCountryList()
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetCountryData();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
       public async Task<List<StateList>> GetStateList(string country,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetStateData(country,site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<StateList>> GetStateListSolar(string country,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetStateDataSolar(country,site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SPVList>> GetSPVList( string state, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSPVData(state,site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SPVList>> GetSPVListSolar(string state,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSPVDataSolar(state,site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindSiteMaster>> GetSiteList(string state, string spvdata,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSiteData(state,spvdata, site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarSiteMaster>> GetSiteListSolar(string state, string spvdata, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSiteDataSolar(state, spvdata, site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindLocationMaster>> GetWTGList(string siteid)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWTGData(siteid);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarLocationMaster>> GetInvList(string siteid, string state, string spv)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetInvData(siteid, state, spv);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindUploadingFilegeneration1>> GetImportGenData(int importId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetImportGenData(importId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindUploadingFileBreakDown1>> GetBrekdownImportData(int importId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetBrekdownImportData(importId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<SolarUploadingFileGeneration2>> GetSolarImportGenData(int importId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarImportGenData(importId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarUploadingFileBreakDown1>> GetSolarBrekdownImportData(int importId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarBrekdownImportData(importId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarUploadingPyranoMeter1Min_1>> GetSolarP1ImportData(int importId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarP1ImportData(importId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarUploadingPyranoMeter15Min_1>> GetSolarP15ImportData(int importId)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarP15ImportData(importId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<int> GetBatchStatus(int import_type, int site_id, string import_date)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetBatchStatus(import_type, site_id, import_date);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BatchIdImport> GetBatchId(string logFileName)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetBatchId(logFileName);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<int> DeleteRecordsAfterFailure(int batchId, int siteType)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.DeleteRecordsAfterFailure(batchId, siteType);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<WindOpertionalHead>> GetOperationHeadData(string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetOperationHeadData(site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<int> DeleteWindSite( int siteid)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.DeleteWindSite(siteid);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<int> DeleteSolarSite(int siteid)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.DeleteSolarSite(siteid);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
		 public async Task<List<SolarOpertionalHead>> GetSolarOperationHeadData(string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetSolarOperationHeadData(site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<WindUploadingFileBreakDown>> GetWindMajorBreakdown(string fromDate, string toDate,string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetWindMajorBreakdown(fromDate, toDate, site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<SolarOpertionalHead1>> GetTotalMWforDashbord(string w_site, string s_site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.GetTotalMWforDashbord(w_site, s_site);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
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

        public async Task<string> EmailWindReport(string fy, string fromDate, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.EmailWindReport(fy, fromDate, site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<string> EmailSolarReport(string fy, string fromDate, string site)
        {
            try
            {
                using (var repos = new DGRRepository(getDB))
                {
                    return await repos.EmailSolarReport(fy, fromDate, site);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
