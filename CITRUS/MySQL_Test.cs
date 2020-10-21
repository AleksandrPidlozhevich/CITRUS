using Autodesk.Revit.DB;
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

            MySqlDataAdapter adapter = new MySqlDataAdapter();

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

                openConnection();
                if (comand.ExecuteNonQuery() == 0)
                {
                    TaskDialog.Show("RevitDB", "Выгрузка данных не успешна");
                }
                closeConnection();
            }

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
