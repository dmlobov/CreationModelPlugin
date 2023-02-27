using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreationModelPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreationModel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Level level1 = GetAddLevel(doc, "Уровень 1");
            Level level2 = GetAddLevel(doc, "Уровень 2");

            AddWall(doc, level1, 10000, 5000);      
           
            return Result.Succeeded;
        }

        public Level GetAddLevel(Document doc, string message)
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
               .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();
            Level level = listLevel
                .Where(x => x.Name.Equals(message))
                .FirstOrDefault();
            return level;
           
        }

        private void AddWall(Document doc, Level level1, double width, double depth)
        {
            double widthValue = UnitUtils.ConvertToInternalUnits(width, DisplayUnitType.DUT_MILLIMETERS);
            double depthValue = UnitUtils.ConvertToInternalUnits(depth, DisplayUnitType.DUT_MILLIMETERS);
            double dx = widthValue / 2;
            double dy = depthValue / 2;
          

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));

            List<Wall> walls = new List<Wall>();

            Transaction transaction = new Transaction(doc, "Построение стен");
            transaction.Start();

            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);
                walls.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level1.Id);
            }
            transaction.Commit();            
        }       
    }
}
