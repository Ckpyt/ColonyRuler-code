﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace ExcelLoading {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="ExcelLoading")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="ExcelLoading", IsNullable=false)]
    public partial class effect {
        
        private Effect[] repetativeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("repetative")]
        public Effect[] repetative {
            get {
                return this.repetativeField;
            }
            set {
                this.repetativeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class Effect {
        
        private string nameField;
        
        private string typeField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AbstractItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(MaterialItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ArmyItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ItemItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BuildingItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Process))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ScienceItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Resource))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class AbstractObject {
        
        private string nameField;
        
        private string descriptionField;
        
        private float defaultXField;
        
        private float defaultYField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public float defaultX {
            get {
                return this.defaultXField;
            }
            set {
                this.defaultXField = value;
            }
        }
        
        /// <remarks/>
        public float defaultY {
            get {
                return this.defaultYField;
            }
            set {
                this.defaultYField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(MaterialItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ArmyItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ItemItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BuildingItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Process))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ScienceItem))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class AbstractItem : AbstractObject {
        
        private string produce_per_personField;
        
        private string dependencyField;
        
        private float dependency_costField;
        
        private float final_costField;
        
        /// <remarks/>
        public string produce_per_person {
            get {
                return this.produce_per_personField;
            }
            set {
                this.produce_per_personField = value;
            }
        }
        
        /// <remarks/>
        public string dependency {
            get {
                return this.dependencyField;
            }
            set {
                this.dependencyField = value;
            }
        }
        
        /// <remarks/>
        public float dependency_cost {
            get {
                return this.dependency_costField;
            }
            set {
                this.dependency_costField = value;
            }
        }
        
        /// <remarks/>
        public float final_cost {
            get {
                return this.final_costField;
            }
            set {
                this.final_costField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ArmyItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ItemItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BuildingItem))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Process))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class MaterialItem : AbstractItem {
        
        private float containerField;
        
        private string container_typeField;
        
        private string production_typeField;
        
        /// <remarks/>
        public float container {
            get {
                return this.containerField;
            }
            set {
                this.containerField = value;
            }
        }
        
        /// <remarks/>
        public string container_type {
            get {
                return this.container_typeField;
            }
            set {
                this.container_typeField = value;
            }
        }
        
        /// <remarks/>
        public string production_type {
            get {
                return this.production_typeField;
            }
            set {
                this.production_typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class ArmyItem : MaterialItem {
        
        private decimal attackField;
        
        private decimal protectionField;
        
        private decimal hitsField;
        
        private decimal distanceField;
        
        private float speed_per_secondField;
        
        /// <remarks/>
        public decimal attack {
            get {
                return this.attackField;
            }
            set {
                this.attackField = value;
            }
        }
        
        /// <remarks/>
        public decimal protection {
            get {
                return this.protectionField;
            }
            set {
                this.protectionField = value;
            }
        }
        
        /// <remarks/>
        public decimal hits {
            get {
                return this.hitsField;
            }
            set {
                this.hitsField = value;
            }
        }
        
        /// <remarks/>
        public decimal distance {
            get {
                return this.distanceField;
            }
            set {
                this.distanceField = value;
            }
        }
        
        /// <remarks/>
        public float speed_per_second {
            get {
                return this.speed_per_secondField;
            }
            set {
                this.speed_per_secondField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BuildingItem))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class ItemItem : MaterialItem {
        
        private float bugField;
        
        private string criticalField;
        
        private float bug_fixing_per_secondField;
        
        private float effectivityField;
        
        private float increasingField;
        
        private string effect_typeField;
        
        /// <remarks/>
        public float bug {
            get {
                return this.bugField;
            }
            set {
                this.bugField = value;
            }
        }
        
        /// <remarks/>
        public string critical {
            get {
                return this.criticalField;
            }
            set {
                this.criticalField = value;
            }
        }
        
        /// <remarks/>
        public float bug_fixing_per_second {
            get {
                return this.bug_fixing_per_secondField;
            }
            set {
                this.bug_fixing_per_secondField = value;
            }
        }
        
        /// <remarks/>
        public float effectivity {
            get {
                return this.effectivityField;
            }
            set {
                this.effectivityField = value;
            }
        }
        
        /// <remarks/>
        public float increasing {
            get {
                return this.increasingField;
            }
            set {
                this.increasingField = value;
            }
        }
        
        /// <remarks/>
        public string effect_type {
            get {
                return this.effect_typeField;
            }
            set {
                this.effect_typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class BuildingItem : ItemItem {
        
        private decimal happyField;
        
        /// <remarks/>
        public decimal happy {
            get {
                return this.happyField;
            }
            set {
                this.happyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class Process : MaterialItem {
        
        private int durationField;
        
        /// <remarks/>
        public int duration {
            get {
                return this.durationField;
            }
            set {
                this.durationField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class ScienceItem : AbstractItem {
        
        private string opensField;
        
        private string closesField;
        
        /// <remarks/>
        public string opens {
            get {
                return this.opensField;
            }
            set {
                this.opensField = value;
            }
        }
        
        /// <remarks/>
        public string closes {
            get {
                return this.closesField;
            }
            set {
                this.closesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="ExcelLoading")]
    public partial class Resource : AbstractObject {
        
        private decimal maximum_in_territoryField;
        
        private float growing_percentField;
        
        /// <remarks/>
        public decimal maximum_in_territory {
            get {
                return this.maximum_in_territoryField;
            }
            set {
                this.maximum_in_territoryField = value;
            }
        }
        
        /// <remarks/>
        public float growing_percent {
            get {
                return this.growing_percentField;
            }
            set {
                this.growing_percentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="ExcelLoading")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="ExcelLoading", IsNullable=false)]
    public partial class resource {
        
        private Resource[] repetativeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("repetative")]
        public Resource[] repetative {
            get {
                return this.repetativeField;
            }
            set {
                this.repetativeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="ExcelLoading")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="ExcelLoading", IsNullable=false)]
    public partial class science {
        
        private ScienceItem[] repetativeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("repetative")]
        public ScienceItem[] repetative {
            get {
                return this.repetativeField;
            }
            set {
                this.repetativeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="ExcelLoading")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="ExcelLoading", IsNullable=false)]
    public partial class materials {
        
        private MaterialItem[] repetativeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("repetative")]
        public MaterialItem[] repetative {
            get {
                return this.repetativeField;
            }
            set {
                this.repetativeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="ExcelLoading")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="ExcelLoading", IsNullable=false)]
    public partial class process {
        
        private Process[] repetativeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("repetative")]
        public Process[] repetative {
            get {
                return this.repetativeField;
            }
            set {
                this.repetativeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="ExcelLoading")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="ExcelLoading", IsNullable=false)]
    public partial class items {
        
        private ItemItem[] repetativeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("repetative")]
        public ItemItem[] repetative {
            get {
                return this.repetativeField;
            }
            set {
                this.repetativeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="ExcelLoading")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="ExcelLoading", IsNullable=false)]
    public partial class buildings {
        
        private BuildingItem[] repetativeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("repetative")]
        public BuildingItem[] repetative {
            get {
                return this.repetativeField;
            }
            set {
                this.repetativeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="ExcelLoading")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="ExcelLoading", IsNullable=false)]
    public partial class army {
        
        private ArmyItem[] repetativeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("repetative")]
        public ArmyItem[] repetative {
            get {
                return this.repetativeField;
            }
            set {
                this.repetativeField = value;
            }
        }
    }
}
