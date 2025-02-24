using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data
{
    public class CheckInData : ICheckInData
    {
        private readonly ISqlDataAccess _db;
        /// <summary>
        /// Holds default connection string name. Must go to one place - GlobalSettings.
        /// </summary>
        private const string connectionStringName = "Testing";

        public CheckInData(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aliasId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">For invalid aliasId.</exception>
        public async Task<StaffBasicModel> GetBasicStaffByAliasId(int aliasId)
        {
            List<StaffBasicModel> output = await _db.LoadData<StaffBasicModel, dynamic>("spStaffs_GetBasicByAliasId",
                                                                                     new { aliasId },
                                                                                     connectionStringName);
            if (output.FirstOrDefault() == null)
                throw new ArgumentException("You passed in an invalid parameter", "aliasId");

            return output.FirstOrDefault();
        }
    }
}
