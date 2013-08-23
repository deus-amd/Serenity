﻿using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public abstract partial class EntityDialog<TEntity, TOptions> : TemplatedDialog<TOptions>
        where TEntity : class, new()
        where TOptions: class, new()
    {
        private TEntity entity;
        private Int64? entityId;
        protected TabsObject tabs;

        protected EntityDialog(TOptions opt)
            : base(Q.NewBodyDiv(), opt)
        {
            InitInferences();
            InitTabs();
            InitToolbar();
            InitPropertyGrid();
            InitLocalizationGrid();
        }

        public override void Destroy()
        {
            if (tabs != null)
                tabs.Destroy();

            if (toolbar != null)
            {
                toolbar.Destroy();
                toolbar = null;
            }

            if (propertyGrid != null)
            {
                propertyGrid.Destroy();
                propertyGrid = null;
            }

            if (localizationGrid != null)
            {
                localizationGrid.Destroy();
                localizationGrid = null;
            }

            if (validator != null)
            {
                this.ById("Form").Remove();
                validator = null;
            }

            this.undeleteButton = null;
            this.saveButton = null;
            this.deleteButton = null;
            this.saveAndCloseButton = null;

            base.Destroy();
        }

        protected virtual void InitTabs()
        {
            var tabsDiv = this.ById("Tabs");
            if (tabsDiv.Length == 0)
                return;

            tabs = tabsDiv.Tabs(new TabsOptions());
        }

        protected TEntity Entity
        {
            get { return entity; }
            set { entity = value ?? new TEntity(); }
        }

        protected internal Nullable<Int64> EntityId
        {
            get { return entityId; }
            set { entityId = value; }
        }

        protected virtual string GetLocalTextPrefix()
        {
            return "Db." + entityType.Value + ".";
        }

        protected virtual string GetEntityNameFieldValue()
        {
            return (Type.GetField(Entity, entityNameField.Value) ?? "").ToString();
        }

        protected virtual string GetEntityTitle()
        {
            if (!(this.IsEditMode))
                return "Yeni " + entitySingular.Value;
            else
            {
                string title = (GetEntityNameFieldValue() ?? "") + " - #" + (this.EntityId.As<long>().ToString());
                return entitySingular.Value + " Düzenle (" + title + ")";
            }
        }

        protected virtual void UpdateTitle()
        {
            element.Dialog().Title = GetEntityTitle();
        }

        protected override void OnDialogOpen()
        {
            if (tabs != null)
                tabs.Active = 0;
        }

        /*
        protected void SetInputsDisabled(Element container, bool isDisabled)
        {
            Query inputs = J.Query(container)
                .find(":input")
                .not(".readonly");

            if (isDisabled)
                inputs.attr("disabled", "disabled");
            else
                inputs.removeAttr("disabled");
        }*/

        protected override string GetTemplateName()
        {
            var templateName = base.GetTemplateName();
            
            if (!Q.CanLoadScriptData("Template." + templateName) &&
                Q.CanLoadScriptData("Template.EntityDialog"))
                return "EntityDialog";

            return templateName;
        }

        protected bool IsCloneMode
        {
            get { return EntityId != null && IdExtensions.IsNegativeId(EntityId.Value); }
        }

        protected bool IsEditMode
        {
            get { return EntityId != null && IdExtensions.IsPositiveId(EntityId.Value); }
        }

        protected bool IsDeleted
        {
            get 
            { 
                if (EntityId == null)
                    return false;

                var value = Type.GetField(Entity, entityIsActiveField.Value).As<Int32?>();
                if (value == null)
                    return false;

                return IdExtensions.IsNegativeId(value.Value);
            }
        }

        protected bool IsNew
        {
            get { return EntityId == null; }
        }

        protected bool IsNewOrDeleted
        {
            get { return IsNew || this.IsDeleted; }
        }
    }
}