using Microsoft.Data.SqlClient;
using System.Data;

namespace KafkaClassLibrary
{
    public class SqlDBHelper
    {
        public static string CONNECTION_STRING = "";
        const Int32 CONNECTION_TIMEOUT = 3000000;

        // This function will be used to execute R(CRUD) operation of parameterless commands
        public static DataTable ExecuteSelectCommand(string CommandName, CommandType cmdType)
        {
            DataTable table;
            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.CommandTimeout = CONNECTION_TIMEOUT;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            table = new DataTable();
                            da.Fill(table);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        // This function will be used to execute R(CRUD) operation of parameterized commands
        public static DataTable ExecuteParameterizedSelectCommand(string CommandName, CommandType cmdType, SqlParameter[] param)
        {
            DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(param);
                    cmd.CommandTimeout = CONNECTION_TIMEOUT;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(table);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        // This function will be used to execute CUD(CRUD) operation of parameterized commands
        public static bool ExecuteNonQuery(string CommandName, CommandType cmdType, SqlParameter[] param)
        {
            int result = 0;

            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(param);
                    cmd.CommandTimeout = CONNECTION_TIMEOUT;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        result = cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return (result > 0);
        }

        public static DataSet ExecuteParameterizedSelectCommandDs(string CommandName, CommandType cmdType, SqlParameter[] param)
        {
            DataSet ds = new DataSet();

            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(param);
                    cmd.CommandTimeout = CONNECTION_TIMEOUT;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return ds;
        }

        public static DataSet ExecuteSelectCommandDs(string CommandName, CommandType cmdType)
        {
            DataSet ds = new DataSet();

            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.CommandTimeout = CONNECTION_TIMEOUT;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return ds;
        }

        public static async Task<DataTable> ExecuteSelectCommandAsync(string CommandName, CommandType cmdType)
        {
            DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.CommandTimeout = CONNECTION_TIMEOUT;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            await con.OpenAsync();
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            await Task.Run(() => da.Fill(table));
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        public static async Task<DataTable> ExecuteParameterizedSelectCommandAsync(string CommandName, CommandType cmdType, SqlParameter[] param)
        {
            DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(param);
                    cmd.CommandTimeout = CONNECTION_TIMEOUT;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            await con.OpenAsync();
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            await Task.Run(() => da.Fill(table));
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return table;
        }

        public static async Task<bool> ExecuteNonQueryAsync(string CommandName, CommandType cmdType, SqlParameter[] param)
        {
            int result = 0;

            using (SqlConnection con = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = cmdType;
                    cmd.CommandText = CommandName;
                    cmd.Parameters.AddRange(param);
                    cmd.CommandTimeout = CONNECTION_TIMEOUT;

                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            await con.OpenAsync();
                        }

                        result = await cmd.ExecuteNonQueryAsync();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return (result > 0);
        }
    }
}
