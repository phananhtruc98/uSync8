﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Entities;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using uSync8.BackOffice.Services;
using uSync8.Core;
using uSync8.Core.Serialization;
using uSync8.Core.Serialization.Serializers;

namespace uSync8.BackOffice.SyncHandlers.Handlers
{
    [SyncHandler("contentTypeHandler", "ContentType Handler", "ContentTypes", 1, IsTwoPass = true)]
    public class ContentTypeHandler : SyncHandlerEntityBase<IContentType>, ISyncHandler
    {
        private readonly IContentTypeService contentTypeService;

        public ContentTypeHandler(
            IEntityService entityService,
            IProfilingLogger logger,
            IContentTypeService contentTypeService,
            ISyncSerializer<IContentType> serializer,
            SyncFileService fileService,
            uSyncBackOfficeSettings settings)
            : base(entityService, logger, serializer, fileService, settings)
        {
            this.contentTypeService = contentTypeService;

            this.itemObjectType = UmbracoObjectTypes.DocumentType;
            this.itemContainerType = UmbracoObjectTypes.DocumentTypeContainer;

        }


        #region Import
            // default import behavior is in the base class.
        #endregion

        #region Export
            // most of export is now in the base 


        #endregion

        #region Reporting
        public override uSyncAction ReportItem(string file)
        {
            return new uSyncAction();
        }
        #endregion

        #region HelperFunctions

        protected override IContentType GetFromService(int id)
            => contentTypeService.Get(id);

        protected override string GetItemFileName(IUmbracoEntity item)
            => item.Name;

        #endregion


        #region Event Handlers 
        public void InitializeEvents()
        {
            ContentTypeService.Saved += ContentTypeService_Saved;
            ContentTypeService.Deleted += ContentTypeService_Deleted;
        }

        private void ContentTypeService_Saved(IContentTypeService sender, Umbraco.Core.Events.SaveEventArgs<IContentType> e)
        {
            // do the save thing....
            foreach (var item in e.SavedEntities)
            {
                Export(item, this.DefaultFolder);
            }
        }

        private void ContentTypeService_Deleted(IContentTypeService sender, Umbraco.Core.Events.DeleteEventArgs<IContentType> e)
        {
            // remove the files from the disk, put something in the action log ?
        }

        #endregion

    }
}
