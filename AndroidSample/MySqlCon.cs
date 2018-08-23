using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinScanditSDKSampleAndroid
{
    public class MySqlCon
    {
        //MySqlConnection conexaoMySQL;
        //MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();

        //public string CriarStringConexao()
        //{
        //    builder.Server = "nicholastcc.database.windows.net";
        //    builder.Database = "SmartFarm";
        //    builder.UserID = "CowAdmin";
        //    builder.Password = "Nich1234";

        //    return builder.ToString();
        //}

        //public int ProducaoDia(float Quant, int IDVaca)
        //{
        //    string StringConexaoMysql = builder;
        //    conexaoMySQL.Open();


        //    try
        //    {
        //        DateTime Data = DateTime.Today.ToLocalTime();
        //        string inserir = "insert into Producao (Periodo, Qtd, IDAnimal) values (" + Data + "," + Quant + "," + IDVaca + ",";
        //        conexaoMySQL = new MySqlConnection(StringConexaoMysql);
        //        conexaoMySQL.Open();

        //        MySqlCommand cmd = new MySqlCommand(inserir, conexaoMySQL);
        //        return (1);

        //    }

        //    catch (Exception ex)
        //    {
        //        return (0);
        //        throw ex;
        //    }
        //    finally
        //    {
        //        conexaoMySQL.Close();
        //    }
        //}
    }
}