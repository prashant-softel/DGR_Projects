using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;

namespace DGRA_V1.Models
{
    public class SolarUploadingFileValidation
    {
        ErrorLog m_ErrorLog;
        List<string> eqList = new List<string> { };

        // Adding key/value pair
        // in the hashtable
        // Using Add() method
        //Console.WriteLine("Key and Value pairs from my_hashtable1:");
        //foreach(DictionaryEntry ele1 in my_hashtable1)
        //{
        //    Console.WriteLine("{0} and {1} ", ele1.Key, ele1.Value);
        //}

        public SolarUploadingFileValidation()
        { }
        ~SolarUploadingFileValidation()
        { }

        public SolarUploadingFileValidation(ErrorLog arErrorLog)
        {
            m_ErrorLog = arErrorLog;
            DataTable dTable = new DataTable();
            var url = "http://localhost:23835/api/DGR/GetSolarLocationMaster";
            var result = string.Empty;

            WebRequest request = WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }

            foreach (DataRow dr in dTable.Rows)
            {
                eqList.Add((string)dr["icr_inv"]);
            }
        }

        public bool validateGenerationData(long rowNumber, string verifyDate, string verifyEq, decimal verifyPowerA, decimal verifyPowerB)
        {
            //Here flags are error flags which are assigned true if the validation condition is not satisfied
            bool todayFlag = false;
            bool eqFlag = false;
            bool powerAFlag = false;
            bool powerBFlag = false;

            //Date Validation CodeBlock
            //DateTime today = DateTime.Now;
            //int weekDay = (int)myDate.DayOfWeek; // 5 due to Friday  

            //bool isEqual = dt.DayOfWeek == DayOfWeek.Thursday);

            ////first see if verifydate is prev day
            ////ok

            ////ifnot
            //int weekDay = (int)myDate.DayOfWeek; // 5 due to Friday  

            //int verifyWeekDay = (int)verifyDate.DayOfWeek;

            //bool isEqual = dt.DayOfWeek == DayOfWeek.Monday);
            //if(isEqual)
            //{
            //    //verifyDate can be two days before
            //    System.TimeSpan diff2 = today - verifyDate;
            //    datedifference diff2 can be 2
            //        then ok //if today is mon

            //}

            //still out of range
            //then check for holidays

            //if ((verifyDate == today))
            //{
            //    todayFlag = true;
            //}


            //Equipment Validation CodeBlock
            if (!(eqList.Contains(verifyEq)))
            {
                eqFlag = true;
            }

            //Inv Power Validation CodeBlock
            string emptyValA = Convert.ToString(verifyPowerA);
            if ((verifyPowerA < 0) || (string.IsNullOrEmpty(emptyValA)))
            {
                powerAFlag = true;
            }

            //Plant Power Validation CodeBlock
            string emptyValB = Convert.ToString(verifyPowerB);
            if ((verifyPowerB < 0) || (string.IsNullOrEmpty(emptyValB)))
            {
                powerBFlag = true;
            }

            //*Overall Error Status*
            if (todayFlag == true || powerAFlag == true || powerBFlag == true || eqFlag == true)
            {
                m_ErrorLog.SetError(",File Row (" + rowNumber + ") had error(s) ");

                if (todayFlag == true)
                {
                    m_ErrorLog.SetError(",Testing Data Submitted On Same Day :" + verifyDate);
                }
                if (eqFlag == true)
                {
                    m_ErrorLog.SetError(",Invalid Equipment Name :" + verifyEq);
                }
                if (powerAFlag == true)
                {
                    m_ErrorLog.SetError(",Inv Power Reading Blank/Negative :" + verifyPowerA);
                }
                if (powerBFlag == true)
                {
                    m_ErrorLog.SetError(",Plant Power Reading Blank/Negative :" + verifyPowerB);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public string breakDownCalc(string stopFrom, string stopTo)
        {
            DateTime stopFrom_ = Convert.ToDateTime(stopFrom);
            DateTime stopTo_ = Convert.ToDateTime(stopTo);
            TimeSpan totalStop_ = stopTo_ - stopFrom_;
            string totalStop = Convert.ToString(totalStop_);
            return totalStop;
        }

        public bool validateBreakDownData(long rowNumber, string stopFrom, string stopTo, string igbd)
        {
            bool greaterStopTo = false;
            //bool lastStopTo = false;
            bool totalBd = false;
            //bool sumOfBDHours = false;

            //1)(Stop To) – column always greater than (Stop From) – column.
            DateTime stopTo_ = Convert.ToDateTime(stopTo);
            DateTime stopFrom_ = Convert.ToDateTime(stopFrom);
            if (stopFrom_> stopTo_)
            {
                greaterStopTo = true;
            }

            //2)(Stop To) Hours - column 24:00:00 should be 23:59:59
            //if (stopTo =="24:00:00")
            //{
            //    lastStopTo = true;
            //}

            //3)BD Hours should not be more than 24 Hrs.
            TimeSpan bdHours = (stopTo_ - stopFrom_);
            if (bdHours.Hours>12)
            {
                totalBd = true;
            }

            //4)Production_hrs+Lull Hrs+USMH+SMH+Others Hour+IGBD+EGBD+LoadShedding = 24 Hrs;
            //Production_hrs,Lull Hrs, USMH, SMH, Others Hour,IGBD,EGBD,LoadShedding should not be negative.
            //if ()
            //{
            //}

            //*Overall Error Status*
            if (greaterStopTo == true || totalBd == true )
            {
                //|| sumOfBDHours == true
                m_ErrorLog.SetError(",File Row (" + rowNumber + ") had error(s) ");

                if (greaterStopTo == true)
                {
                    m_ErrorLog.SetError(",Stop-To Time is lower than Stop-From: ");
                }
                //if (lastStopTo == true)
                //{
                //    m_ErrorLog.SetError(",Invalid  Stop-To timing:");
                //}
                if (totalBd == true)
                {
                    m_ErrorLog.SetError(",Breakdown Hours are exceeding 24 hours:");
                }
                //if (sumOfBDHours == true)
                //{
                //    m_ErrorLog.SetError(",Sum of all breakdown hours exceed 24 hours: ");
                //}
                return true;
            }
            else
            {
                return false;
            }
        }

        
    }
}


