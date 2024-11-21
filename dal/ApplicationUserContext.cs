using backend_devops_rejsekort_v2.dto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend_devops_rejsekort_v2.dal
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<LocationPair> LocationPairs { get; set; } = new List<LocationPair>();
    }
}
