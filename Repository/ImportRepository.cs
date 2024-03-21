using System.Data.SqlClient;
using System.Text;
using BrainBoost.Parameter;
using Dapper;

namespace BrainBoost.Services
{
    public class ImportRepository
    {
        #region 呼叫函式
        private readonly string? cnstr;

        public ImportRepository(IConfiguration configuration){
            cnstr = configuration.GetConnectionString("ConnectionStrings");
        }
        #endregion
        
        
    }
}
