using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NOC_Macro.Models
{
    [MetadataType(typeof(MajorIncidentsMetaData))]
    public partial class MajorIncidents
    {

    }

    public class MajorIncidentsMetaData
    {

        
        [Display(Name = "Incident #")]
        [Required(ErrorMessage = "Incident # is required")]
        [Remote("IsIncidentExists", "Home", ErrorMessage = "User Name already in use")]
        public int incidentNumber { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string descr { get; set; }

        [Display(Name = "Product List")]
        [Required(ErrorMessage = "Product list is required")]
        public string product { get; set; }

        [Display(Name = "Datacenter list")]
        [Required(ErrorMessage = "DataCenter list is required")]
        public string dataCenter { get; set; }

        [Display(Name = "Incident Type")]
        public int categorization { get; set; }

        [Display(Name = "Customer Type")]
        public int customerType { get; set; }
    }
}