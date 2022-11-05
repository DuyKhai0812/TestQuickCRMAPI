using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace TestQuickApp.Controllers.ExecuteNonQuery
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecuteNonQueryController : ControllerBase
    {


        private DataTable DTExecuteNonQuery(string query, params object[] param)
        {

            DataTable table = new DataTable();
            using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlCommandBuilder.DeriveParameters(cmd);
                    cmd.Parameters.RemoveAt(0);
                    SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];
                    cmd.Parameters.CopyTo(discoveredParameters, 0);

                    AssignParameterValues(discoveredParameters, param);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(table);
                    }
                }
                con.Close();
            }
            return table;
        }

        private DataSet DSExecuteNonQuery(string query, params object[] param)
        {
            DataSet ds = new DataSet();
            using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    SqlCommandBuilder.DeriveParameters(cmd);
                    cmd.Parameters.RemoveAt(0);
                    SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];
                    cmd.Parameters.CopyTo(discoveredParameters, 0);

                    AssignParameterValues(discoveredParameters, param);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }

            //iterate through the SqlParameters, assigning the values from the corresponding position in the
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                if ((parameterValues.Length - 1 < i) || parameterValues[i] == null)
                    commandParameters[i].Value = DBNull.Value;
                else
                    commandParameters[i].Value = parameterValues[i];
            }
        }
    }
}
