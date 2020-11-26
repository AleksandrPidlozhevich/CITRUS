using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

        public class App : IExternalApplication
        {
            public Result OnShutdown(UIControlledApplication application)
            {
                return Result.Succeeded;
            }

            public Result OnStartup(UIControlledApplication application)
            {
            // Создание панели
            string assemblyPach = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string tabName = "ЦИТRUS";
            application.CreateRibbonTab(tabName);

            //Создание категории кнопок "Публикация"
            RibbonPanel panel = application.CreateRibbonPanel(tabName, "Публикация");

            //Создание кнопки "Собиратор" в категории "Публикация"
            PushButtonData pbd = new PushButtonData("Sobirator", "Собиратор", assemblyPach, "CITRUS.Sobirator");

            Image img1 = Properties.Resources.Sobirator_Large;
            ImageSource imgLarge = GetImageSourse(img1);
            Image img2 = Properties.Resources.Sobirator;
            ImageSource imgStandart = GetImageSourse(img2);

            pbd.LargeImage = imgLarge;
            pbd.Image = imgStandart;
            panel.AddItem(pbd);

            //Создание кнопки "TXTExport" в категории "Публикация"
            PushButtonData pbdTXTExport = new PushButtonData("TXTExport", "TXT\nЭкспорт", assemblyPach, "CITRUS.TXTExport");
            Image TXTExport_img1 = Properties.Resources.ScheduleTXTExport_Large;
            ImageSource TXTExport_imgLarge = GetImageSourse(TXTExport_img1);
            Image TXTExport_img2 = Properties.Resources.ScheduleTXTExport;
            ImageSource TXTExport_imgStandart = GetImageSourse(TXTExport_img2);

            pbdTXTExport.LargeImage = TXTExport_imgLarge;
            pbdTXTExport.Image = TXTExport_imgStandart;
            panel.AddItem(pbdTXTExport);


            //Создание категории кнопок "АР"
            RibbonPanel panel_AR = application.CreateRibbonPanel(tabName, "АР");

            //Создание кнопки "Генератор пола" в категории "Публикация"
            PushButtonData pbdFloorGenerator = new PushButtonData("FloorGenerator"
                , "Генератор\nпола"
                , assemblyPach
                , "CITRUS.FloorGenerator");

            Image FloorGenerator_img1 = Properties.Resources.FloorGenerator_Large;
            ImageSource FloorGenerator_imgLarge = GetImageSourse(FloorGenerator_img1);
            Image FloorGenerator_img2 = Properties.Resources.FloorGenerator;
            ImageSource FloorGenerator_imgStandart = GetImageSourse(FloorGenerator_img2);

            pbdFloorGenerator.LargeImage = FloorGenerator_imgLarge;
            pbdFloorGenerator.Image = FloorGenerator_imgStandart;
            panel_AR.AddItem(pbdFloorGenerator);

            //Создание кнопки "WallFinishCreator" в категории "АР"
            PushButtonData pbdWallFinishCreator = new PushButtonData("WallFinishCreator"
                    , "Отделка\nстен"
                    , assemblyPach
                    , "CITRUS.CIT_03_1_WallFinishCreator.CIT_03_1_WallFinishCreator");

            Image WallFinishCreator_img1 = Properties.Resources.WallFinishCreator_Large;
            ImageSource WallFinishCreator_imgLarge = GetImageSourse(WallFinishCreator_img1);
            Image WallFinishCreator_img2 = Properties.Resources.WallFinishCreator;
            ImageSource WallFinishCreator_imgStandart = GetImageSourse(WallFinishCreator_img2);

            pbdWallFinishCreator.LargeImage = WallFinishCreator_imgLarge;
            pbdWallFinishCreator.Image = WallFinishCreator_imgStandart;
            panel_AR.AddItem(pbdWallFinishCreator);

            //Создание кнопки "FinishNumerator" в категории "АР"
            PushButtonData pbdFinishNumerator = new PushButtonData("FinishNumerator"
                    , "Нумератор\nотделки"
                    , assemblyPach
                    , "CITRUS.CIT_03_2_FinishNumerator.CIT_03_2_FinishNumerator");

            Image FinishNumerator_img1 = Properties.Resources.FinishNumerator_Large;
            ImageSource FinishNumerator_imgLarge = GetImageSourse(FinishNumerator_img1);
            Image FinishNumerator_img2 = Properties.Resources.FinishNumerator;
            ImageSource FinishNumerator_imgStandart = GetImageSourse(FinishNumerator_img2);

            pbdFinishNumerator.LargeImage = FinishNumerator_imgLarge;
            pbdFinishNumerator.Image = FinishNumerator_imgStandart;
            panel_AR.AddItem(pbdFinishNumerator);

            //Создание категории кнопок "КР"
            RibbonPanel panel_KR = application.CreateRibbonPanel(tabName, "КР");

            //Создание кнопки "Капитель" в категории "Публикация"
            PushButtonData pbdCapitalMaker = new PushButtonData("CapitalMaker", "Капитель", assemblyPach, "CITRUS.CapitalMaker");
            Image CapitalMaker_img1 = Properties.Resources.CapitalMaker_Large;
            ImageSource CapitalMaker_imgLarge = GetImageSourse(CapitalMaker_img1);
            Image CapitalMaker_img2 = Properties.Resources.CapitalMaker;
            ImageSource CapitalMaker_imgStandart = GetImageSourse(CapitalMaker_img2);

            pbdCapitalMaker.LargeImage = CapitalMaker_imgLarge;
            pbdCapitalMaker.Image = CapitalMaker_imgStandart;
            panel_KR.AddItem(pbdCapitalMaker);

            //Создание кнопки "Армирование квадратной колонны" в категории "КР"
            PushButtonData pbdSquareColumnsReinforcement = new PushButtonData("SquareColumnsReinforcement"
                , "Арм.Квадратной\nколонны"
                , assemblyPach
                , "CITRUS.CIT_04_1_SquareColumnsReinforcement.CIT_04_1_SquareColumnsReinforcement");

            Image SquareColumnsReinforcement_img1 = Properties.Resources.SquareColumnsReinforcement_Large;
            ImageSource SquareColumnsReinforcement_imgLarge = GetImageSourse(SquareColumnsReinforcement_img1);
            Image SquareColumnsReinforcement_img2 = Properties.Resources.SquareColumnsReinforcement;
            ImageSource SquareColumnsReinforcement_imgStandart = GetImageSourse(SquareColumnsReinforcement_img2);

            pbdSquareColumnsReinforcement.LargeImage = SquareColumnsReinforcement_imgLarge;
            pbdSquareColumnsReinforcement.Image = SquareColumnsReinforcement_imgStandart;
            panel_KR.AddItem(pbdSquareColumnsReinforcement);

            //Создание кнопки "Армирование прямоугольной колонны" в категории "КР"
            PushButtonData pbdRectangularColumnsReinforcement = new PushButtonData("RectangularColumnsReinforcement"
                , "Арм.Прямоугольной\nколонны"
                , assemblyPach
                , "CITRUS.CIT_04_2_RectangularColumnsReinforcement.CIT_04_2_RectangularColumnsReinforcement");

            Image RectangularColumnsReinforcement_img1 = Properties.Resources.RectangularColumnsReinforcement_Large;
            ImageSource RectangularColumnsReinforcement_imgLarge = GetImageSourse(RectangularColumnsReinforcement_img1);
            Image RectangularColumnsReinforcement_img2 = Properties.Resources.RectangularColumnsReinforcement;
            ImageSource RectangularColumnsReinforcement_imgStandart = GetImageSourse(RectangularColumnsReinforcement_img2);

            pbdRectangularColumnsReinforcement.LargeImage = RectangularColumnsReinforcement_imgLarge;
            pbdRectangularColumnsReinforcement.Image = RectangularColumnsReinforcement_imgStandart;
            panel_KR.AddItem(pbdRectangularColumnsReinforcement);

            //Создание кнопки "Выпуски" в категории "КР"
            PushButtonData pbdRebarOutletsCreator = new PushButtonData("RebarOutletsCreator", "Выпуски", assemblyPach, "CITRUS.RebarOutletsCreator");

            Image RebarOutletsCreator_img1 = Properties.Resources.RebarOutletsCreator_Large;
            ImageSource RebarOutletsCreator_imgLarge = GetImageSourse(RebarOutletsCreator_img1);
            Image RebarOutletsCreator_img2 = Properties.Resources.RebarOutletsCreator;
            ImageSource RebarOutletsCreator_imgStandart = GetImageSourse(RebarOutletsCreator_img2);
            pbdRebarOutletsCreator.LargeImage = RebarOutletsCreator_imgLarge;
            pbdRebarOutletsCreator.Image = RebarOutletsCreator_imgStandart;

            panel_KR.AddItem(pbdRebarOutletsCreator);

            //Создание кнопки "Копирователь групп" в категории "КР"
            PushButtonData pbdRebarGroupCopier = new PushButtonData("RebarGroupCopier"
                , "Копирователь\nгрупп"
                , assemblyPach
                , "CITRUS.RebarGroupCopierScript");

            Image RebarGroupCopier_img1 = Properties.Resources.RebarGroupCopier_Large;
            ImageSource RebarGroupCopier_imgLarge = GetImageSourse(RebarGroupCopier_img1);
            Image RebarGroupCopier_img2 = Properties.Resources.RebarGroupCopier;
            ImageSource RebarGroupCopier_imgStandart = GetImageSourse(RebarGroupCopier_img2);

            pbdRebarGroupCopier.LargeImage = RebarGroupCopier_imgLarge;
            pbdRebarGroupCopier.Image = RebarGroupCopier_imgStandart;
            panel_KR.AddItem(pbdRebarGroupCopier);

            //Создание кнопки "BeamReinforcement" в категории "КР"
            PushButtonData pbdBeamReinforcement = new PushButtonData("BeamReinforcement"
                    , "Арм.Балки"
                    , assemblyPach
                    , "CITRUS.CIT_04_3_BeamReinforcement.CIT_04_3_BeamReinforcement");

            Image BeamReinforcement_img1 = Properties.Resources.BeamReinforcement_Large;
            ImageSource BeamReinforcement_imgLarge = GetImageSourse(BeamReinforcement_img1);
            Image BeamReinforcement_img2 = Properties.Resources.BeamReinforcement;
            ImageSource BeamReinforcement_imgStandart = GetImageSourse(BeamReinforcement_img2);

            pbdBeamReinforcement.LargeImage = BeamReinforcement_imgLarge;
            pbdBeamReinforcement.Image = BeamReinforcement_imgStandart;
            panel_KR.AddItem(pbdBeamReinforcement);

            // Создание кнопки "StaircaseReinforcement" в категории "Другое"
            PushButtonData pbdStaircaseReinforcement = new PushButtonData("StairFlightReinforcement"
                    , "Арм.Марш"
                    , assemblyPach
                    , "CITRUS.CIT_04_5_StairFlightReinforcement.CIT_04_5_StairFlightReinforcement");

            Image StaircaseReinforcement_img1 = Properties.Resources.StaircaseReinforcement_Large;
            ImageSource StaircaseReinforcement_imgLarge = GetImageSourse(StaircaseReinforcement_img1);
            Image StaircaseReinforcement_img2 = Properties.Resources.StaircaseReinforcement;
            ImageSource StaircaseReinforcement_imgStandart = GetImageSourse(StaircaseReinforcement_img2);

            pbdStaircaseReinforcement.LargeImage = StaircaseReinforcement_imgLarge;
            pbdStaircaseReinforcement.Image = StaircaseReinforcement_imgStandart;

            panel_KR.AddItem(pbdStaircaseReinforcement);

            //Создание категории кнопок "ОВ"
            RibbonPanel panel_OV = application.CreateRibbonPanel(tabName, "ОВ");
            //Создание кнопки "MEPViewScheduleCreator" в категории "ОВ"
            PushButtonData pbdMEPViewScheduleCreator = new PushButtonData("MEPViewScheduleCreator", "Создать\ncпецификации MEP", assemblyPach, "CITRUS.MEPViewScheduleCreator");

            Image MEPViewScheduleCreator_img1 = Properties.Resources.ScheduleCreator_Large;
            ImageSource MEPViewScheduleCreator_imgLarge = GetImageSourse(MEPViewScheduleCreator_img1);
            Image MEPViewScheduleCreator_img2 = Properties.Resources.ScheduleCreator;
            ImageSource MEPViewScheduleCreator_imgStandart = GetImageSourse(MEPViewScheduleCreator_img2);
            pbdMEPViewScheduleCreator.LargeImage = MEPViewScheduleCreator_imgLarge;
            pbdMEPViewScheduleCreator.Image = MEPViewScheduleCreator_imgStandart;
            panel_OV.AddItem(pbdMEPViewScheduleCreator);

            //Создание кнопки "MEPViewScheduleHost" в категории "ОВ"
            PushButtonData pbdMEPViewScheduleHost = new PushButtonData("MEPViewScheduleHost", "Разместить\nспецификации MEP", assemblyPach, "CITRUS.MEPViewScheduleHost");
            Image MEPViewScheduleHost_img1 = Properties.Resources.ScheduleHost_Large;
            ImageSource MEPViewScheduleHost_imgLarge = GetImageSourse(MEPViewScheduleHost_img1);
            Image MEPViewScheduleHost_img2 = Properties.Resources.ScheduleHost;
            ImageSource MEPViewScheduleHost_imgStandart = GetImageSourse(MEPViewScheduleHost_img2);
            pbdMEPViewScheduleHost.LargeImage = MEPViewScheduleHost_imgLarge;
            pbdMEPViewScheduleHost.Image = MEPViewScheduleHost_imgStandart;
            panel_OV.AddItem(pbdMEPViewScheduleHost);


            //Создание категории кнопок "Другое"
            RibbonPanel panel_Other = application.CreateRibbonPanel(tabName, "Другое");

            //Создание кнопки "VR" в категории "Другое"
            PushButtonData pbdVoiceRecognition = new PushButtonData("VoiceRecognition", "VR", assemblyPach, "CITRUS.VoiceRecognition.VoiceRecognition");
            Image VoiceRecognition_img1 = Properties.Resources.VoiceRecognition_Large;
            ImageSource VoiceRecognition_imgLarge = GetImageSourse(VoiceRecognition_img1);
            Image VoiceRecognition_img2 = Properties.Resources.VoiceRecognition_Small;
            ImageSource VoiceRecognition_imgStandart = GetImageSourse(VoiceRecognition_img2);

            pbdVoiceRecognition.LargeImage = VoiceRecognition_imgLarge;
            pbdVoiceRecognition.Image = VoiceRecognition_imgStandart;
            panel_Other.AddItem(pbdVoiceRecognition);

            //Создание кнопки "GloryHole" в категории "Другое"
            PushButtonData pbdGloryHole = new PushButtonData("GloryHole", "GloryHole", assemblyPach, "CITRUS.GloryHole");
            Image GloryHole_img1 = Properties.Resources.GloryHole_Large;
            ImageSource GloryHole_imgLarge = GetImageSourse(GloryHole_img1);
            Image GloryHole_img2 = Properties.Resources.GloryHole;
            ImageSource GloryHole_imgStandart = GetImageSourse(GloryHole_img2);

            pbdGloryHole.LargeImage = GloryHole_imgLarge;
            pbdGloryHole.Image = GloryHole_imgStandart;
            panel_Other.AddItem(pbdGloryHole);

            //Создание кнопки "MEPViewScheduleCreatorADM" в категории "Другое"
            PushButtonData pbdMEPViewScheduleCreatorADM = new PushButtonData("MEPViewScheduleCreatorADM"
                    , "Создать\ncпецификации MEPADM"
                    , assemblyPach
                    , "CITRUS.MEPViewScheduleCreatorADM");

            panel_Other.AddItem(pbdMEPViewScheduleCreatorADM);

            // Создание кнопки "HeatLoss" в категории "Другое"
            PushButtonData pbdHeatLoss = new PushButtonData("HeatLoss"
                    , "Теплопотери"
                    , assemblyPach
                    , "CITRUS.HeatLoss");

            panel_Other.AddItem(pbdHeatLoss);

            // Создание кнопки "Draftsman" в категории "Другое"
            PushButtonData pbdDraftsman = new PushButtonData("Draftsman"
                    , "Чертило"
                    , assemblyPach
                    , "CITRUS.Draftsman");

            panel_Other.AddItem(pbdDraftsman);

            // Создание кнопки "MySQL_Test" в категории "Другое"
            PushButtonData pbdMySQLTest = new PushButtonData("MySQLTest"
                    , "MySQLTest"
                    , assemblyPach
                    , "CITRUS.MySQL_Test");

            panel_Other.AddItem(pbdMySQLTest);

            // Создание кнопки "Blocker" в категории "Другое"
            PushButtonData pbdBlocker = new PushButtonData("Blocker"
                    , "Blocker"
                    , assemblyPach
                    , "CITRUS.Blocker");

            panel_Other.AddItem(pbdBlocker);
            //CIT_05_4_1_MEPViewScheduleCleaner_Roven
            //  Создание кнопки "MEPViewScheduleCreator_Roven" в категории "Другое"
            PushButtonData pbdMEPViewScheduleCreator_Roven = new PushButtonData("MEPViewScheduleCreator_Roven"
                    , "РОВЕН\nСоздать спецификации"
                    , assemblyPach
                    , "CITRUS.CIT_05_4_1_MEPViewScheduleCreator_Roven.CIT_05_4_1_MEPViewScheduleCreator_Roven");

            panel_Other.AddItem(pbdMEPViewScheduleCreator_Roven);

            //  Создание кнопки "MEPViewScheduleCleaner_Roven" в категории "Другое"
            PushButtonData pbdMEPViewScheduleCleaner_Roven = new PushButtonData("MEPViewScheduleCleaner_Roven"
                    , "РОВЕН\nУдалить спецификации"
                    , assemblyPach
                    , "CITRUS.CIT_05_4_1_MEPViewScheduleCreator_Roven.CIT_05_4_1_MEPViewScheduleCleaner_Roven");

            panel_Other.AddItem(pbdMEPViewScheduleCleaner_Roven);

            //Создание кнопки "HoleTransfer" в категории "Другое"
            PushButtonData pbdHoleTransfer = new PushButtonData("HoleTransfer"
                    , "Перенос\nпроемов"
                    , assemblyPach
                    , "CITRUS.CIT_04_6_HoleTransfer.CIT_04_6_HoleTransfer");

            panel_Other.AddItem(pbdHoleTransfer);

            return Result.Succeeded;
            }
            // Конвертер изображения
            private BitmapSource GetImageSourse(Image img)
            {
                BitmapImage bmp = new BitmapImage();
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    bmp.BeginInit();

                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = null;
                    bmp.StreamSource = ms;

                    bmp.EndInit();
                }
                return bmp;
            }
        }
}
