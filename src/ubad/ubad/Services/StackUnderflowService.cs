using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using ubad.JsonModels;

namespace ubad.Services
{
    public class StackUnderflowService
    {
        public SqlConnection DbConn { get; set; }

        public StackUnderflowService()
        {
            DbConn = new SqlConnection(Environment.GetEnvironmentVariable("SqlServerSprocConnection"));
        }
        internal async Task<QuestionAndAnswer> FtsFtQuery(string text)
        {
            try
            {
                using var command = DbConn.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "usp_TopFtsQuery";
                command.Parameters.AddWithValue("queryText", text);
                var questionAndAnswer = new QuestionAndAnswer();
                DbConn.Open();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        if (reader[0].ToString() == "Question")
                        {
                            questionAndAnswer.Question = reader[1].ToString();
                        }
                        else if (reader[0].ToString() == "Answer")
                        {
                            questionAndAnswer.Answer = reader[1].ToString();
                        }
                    }
                }
                return questionAndAnswer;
            }finally
            {
                DbConn.Close();
            }
        }
    }
}
