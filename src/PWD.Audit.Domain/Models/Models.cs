using PWD.Schedule.Enum;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace PWD.Schedule.Models
{
    public class Chapter : Entity<int>
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int PageFrom { get; set; }
        public int PageTo { get; set; }
        public WorkType Type { get; set; }
    }

    public class Topic : Entity<int>
    {
        public int ChapterId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int? Page { get; set; } = 0;
        public int Sequence { get; set; } = 0;
        public virtual Chapter Chapter { get; set; }
    }

    public class Item : Entity<int>
    {
        public int TopicId { get; set; }
        public int? CategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string Unit { get; set; }
        public double Rate { get; set; }
        public int? Page { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual Category Category { get; set; }
    }

    public class Category : Entity<int>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
    }


    public class Unit : Entity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class EstimateProject : Entity<int>
    {
        public string Name { get; set; }
        public string MeasurementUnit { get; set; }
        public int NumberofBuildings { get; set; }
        public List<EstimateComponent> Components { get; set; }
    }

    public class EstimateComponent : Entity<int>
    {
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public double TotalValue { get; set; }
        public List<EstimateItem> Items { get; set; }
    }

    public class EstimateItem : Entity<int>
    {
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public string Variation { get; set; }
        public WorkType WorkType { get; set; }
        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double TotalValue { get; set; }
        public bool IsAnalysis { get; set; }
        public List<EstimateBlock> Blocks { get; set; }
        public List<EstimateDimension> Dimensions { get; set; }

    }
    public class EstimateBlock : Entity<int>
    {
        public int RefNo { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public string Variation { get; set; }
        public WorkType WorkType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Qurantity { get; set; }
        public double QurantitySI { get; set; }
        public double Rate { get; set; }
        public double TotalValue { get; set; }
        public bool IsDeduction { get; set; }
        public bool IsValue { get; set; }
        public int ParentId { get; set; }
        public string QuantityRef { get; set; }
        public int ItemId { get; set; }
        public EstimateItem Item { get; set; }
        public List<EstimateDimension> Dimensions { get; set; }
    }

    public class EstimateDimension : Entity<int>
    {
        public string Heading { get; set; }
        public string Description { get; set; }
        //Multipliers
        public double Buildings { get; set; } = 1;
        public double Floors { get; set; } = 1;
        public double Quantity { get; set; } = 1;
        //Dimensions
        public double Length { get; set; }
        public double LengthFraction { get; set; } = 0;
        public double Width { get; set; }
        public double WidthFraction { get; set; } = 0;
        public double Height { get; set; }
        public double HeightFraction { get; set; } = 0;
        public double Diameter { get; set; }
        public double DiameterFraction { get; set; } = 0;
        public double Reinforcement { get; set; } = 0;
        //Total
        public double TotalQuantity { get; set; }
        public string Unit { get; set; }
        public MeasurementUnitType MeasurementUnitType { get; set; }
        public ItemType ItemType { get; set; }
        public string QuantityRef { get; set; }
        public int? BlockId { get; set; }
        public EstimateBlock? Block { get; set; }
        public int? ItemId { get; set; }
        public EstimateItem? Item { get; set; }
    }


    public enum ItemType
    {
        Cube = 1,
        Cubid = 2,
        Cylinder = 3,
        Sphere = 4,
        Cone = 5,
        TriangularPrism = 6,
        SquareBasedPyramid = 7,
        TriangularBasedPyramid = 8,
        AreaCubid = 9,
        AreaCubidOpen = 10,
        AreaCylinder = 11,
        Reference = 12,
        ReferenceMultiply = 13,
        RefereneWeight = 14,
    }

    public enum UnitType
    {
        Imperial = 1,
        SI = 2
    }

    public enum BlockType
    {
        Regular = 1, //Simple block set of dimensions
        Parent = 2,
        Child = 3,
        Variation = 4,
        Reference = 5,
        ReferenceWithVariation = 6,
    }
    public enum WorkType
    {
        Civil = 2,
        ElectroMechanical = 1
    }
}
