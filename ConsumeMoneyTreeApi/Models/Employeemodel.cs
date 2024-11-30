using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsumeMoneyTreeApi.Models
{
    public class Employeemodel
    {
        public int iEmpId { get; set; }
        public int iState { get; set; }
        public int iCountryId { get; set; }

        public string sCity { get; set; }

        public string sState { get; set; }
        public string sCountry { get; set; }

        public int iCity { get; set; }
        public string sName { get; set; }
        public string sPhoneNo { get; set; }
        public string sAddress { get; set; }
        public DateTime dtDoB { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }
        public IEnumerable<SelectListItem> States { get; set; }
        public IEnumerable<SelectListItem> Cities { get; set; }
    }

    public class City
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int? StateID { get; set; }
    }

    public class State
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
        public int? CountryIDRef { get; set; }
    }

    public class Country
    {
        public long CountryID { get; set; }
        public string CountryName { get; set; }
    }

}