﻿using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CITRUS.CIT_03_1_WallFinishCreator
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CIT_03_1_WallFinishCreator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIApplication app = commandData.Application;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            RoomSelectionFilter selFilter = new RoomSelectionFilter();
            IList<Reference> selRooms = sel.PickObjects(ObjectType.Element, selFilter, "Выберите помещения!");
            List<Room> roomList = new List<Room>();

            foreach (Reference roomRef in selRooms)
            {
                roomList.Add(doc.GetElement(roomRef) as Room);
            }

            List<WallType> wallTypeFirstList = new FilteredElementCollector(doc).OfClass(typeof(WallType)).Cast<WallType>().ToList();
            
            CIT_03_1_WallFinishCreatorForm wallFinishCreatorForm = new CIT_03_1_WallFinishCreatorForm(wallTypeFirstList);
            wallFinishCreatorForm.ShowDialog();
            if (wallFinishCreatorForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }

            double mainWallFinishHeight = wallFinishCreatorForm.MainWallFinishHeight / 304.8;
            WallType wallTypeFirst = wallFinishCreatorForm.mySelectionWallTypeFirst;
            double wallTypeFirstWidth = wallTypeFirst.Width;
            double wallTypeFirstOffset = 0;

            List<Wall> wallListForMove = new List<Wall>();

            //Транзакция
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Создание отделки");
                foreach (Room myRoom in roomList)
                {


                    List<Curve> roomCurves = new List<Curve>();
                    IList<IList<BoundarySegment>> loops = myRoom.GetBoundarySegments(new SpatialElementBoundaryOptions());
                    foreach (IList<BoundarySegment> loop in loops)
                    {
                        foreach (BoundarySegment seg in loop)
                        {
                            Wall wall = Wall.Create(doc, seg.GetCurve(), wallTypeFirst.Id, myRoom.LevelId, mainWallFinishHeight, wallTypeFirstOffset, false, false);
                            wall.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(3);
                            wall.get_Parameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING).Set(0);
                            wallListForMove.Add(wall);
                        }
                    }
                }
                t.Commit();

                t.Start("Смещение стен");
                foreach(Wall wall in wallListForMove)
                {
                    XYZ wallOrientationVector = wall.Orientation;
                    ElementTransformUtils.MoveElement(doc, wall.Id, wallOrientationVector * (wallTypeFirstWidth / 2));

                    BoundingBoxXYZ bbox = wall.get_BoundingBox(null);
                    Outline myOutLn = new Outline(bbox.Min, bbox.Max);

                    List<Wall> intersectWallList = new FilteredElementCollector(doc)
                        .OfClass(typeof(Wall))
                        .WherePasses(new BoundingBoxIntersectsFilter(myOutLn))
                        .Cast<Wall>()
                        .Where(w => w.WallType.Id != wallTypeFirst.Id)
                        .Where(w => w.WallType.Kind.ToString() != "Curtain")
                        .Where(w => w.get_Parameter(BuiltInParameter.PHASE_DEMOLISHED).AsValueString() == "Нет")
                        .ToList();
                    foreach (Wall w in intersectWallList)
                    {
                        try
                        {
                            JoinGeometryUtils.JoinGeometry(doc, wall, w);
                        }
                        catch (Autodesk.Revit.Exceptions.ArgumentException)
                        {
                            continue;
                        }
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
