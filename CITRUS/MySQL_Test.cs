using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class MySQL_Test : IExternalCommand
    {
        MySqlConnection connection = new MySqlConnection("server = localhost;port=3306;username=root;password=lp254469444;database=revitdb");
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
           
            //Выбор всех колонн в проекте
            List<FamilyInstance> columns = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            if (columns.Count == 0)
            {
                TaskDialog.Show("Revit", "Колонны не найдены");
                return Result.Cancelled;
            }

            List<Rebar> rebars = new FilteredElementCollector(doc).OfClass(typeof(Rebar)).Cast<Rebar>().ToList();
            if (rebars.Count == 0)
            {
                TaskDialog.Show("Revit", "Арматура не найдена");
                return Result.Cancelled;
            }

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            openConnection();
            foreach (FamilyInstance column in columns)
            {
                int columnIdFromRevit = 0;
                Int32.TryParse(column.Id.ToString(), out columnIdFromRevit);
                string columnMarkFromRevit = column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                double columnVolumeFromRevit = column.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() * 0.0283168;

                MySqlCommand comand = new MySqlCommand("INSERT INTO `structural_columns`(`id`,`mark`,`volume`) VALUES(@columnId, @columnMark,@columnVolume);", getConnection());
                comand.Parameters.Add("@columnId", MySqlDbType.Int32).Value = columnIdFromRevit;
                comand.Parameters.Add("@columnMark", MySqlDbType.VarChar).Value = columnMarkFromRevit;
                comand.Parameters.Add("@columnVolume", MySqlDbType.Double).Value = columnVolumeFromRevit;
                adapter.SelectCommand = comand;

                if (comand.ExecuteNonQuery() == 0)
                {
                    TaskDialog.Show("RevitDB", "Выгрузка данных колонн не успешна");
                    return Result.Failed;
                }
            }

            foreach (Rebar reb in rebars)
            {
                int rebarIdFromRevit = 0;
                Int32.TryParse(reb.Id.ToString(), out rebarIdFromRevit);
                int rebarHostIdFromRevit = 0;
                Int32.TryParse(reb.GetHostId().ToString(), out rebarHostIdFromRevit);
                string rebarBarTypeName = doc.GetElement(reb.GetTypeId()).Name;
                double rebarTotalLight = Math.Round(reb.TotalLength*304.8,3);

                MySqlCommand comand = new MySqlCommand("INSERT INTO `rebar` (`id`,`host_id`,`rebar_bar_type_name`,`rebar_total_light`) VALUES (@rebarIdFromRevit,@rebarHostIdFromRevit,@rebarBarTypeName,@rebarTotalLight);", getConnection());
                comand.Parameters.Add("@rebarIdFromRevit", MySqlDbType.Int32).Value = rebarIdFromRevit;
                comand.Parameters.Add("@rebarHostIdFromRevit", MySqlDbType.Int32).Value = rebarHostIdFromRevit;
                comand.Parameters.Add("@rebarBarTypeName", MySqlDbType.VarChar).Value = rebarBarTypeName;
                comand.Parameters.Add("@rebarTotalLight", MySqlDbType.Double).Value = rebarTotalLight;
                adapter.SelectCommand = comand;

                if (comand.ExecuteNonQuery() == 0)
                {
                    TaskDialog.Show("RevitDB", "Выгрузка данных арматуры не успешна");
                    return Result.Failed;
                }
            }

            closeConnection();

            TaskDialog.Show("RevitDB", "Выгрузка завершена");
            return Result.Succeeded;
        }
        public void openConnection()
        {
            if(connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public void closeConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }
    }
}
