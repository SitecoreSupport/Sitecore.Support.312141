﻿﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.PreprocessRequest;

namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Pipelines.PreprocessRequest
{
    public class FilterUrlFilesAndExtensions : FilterUrlExtensions
    {
        private IEnumerable<string> _allowedFileNames;

        protected virtual IEnumerable<string> AllowedFileNames
        {
            get { return _allowedFileNames ?? (_allowedFileNames = GetAllowedFileNames()); }
            set { _allowedFileNames = value; }
        }

        public FilterUrlFilesAndExtensions(string allowed, string blocked, string streamFiles, string doNotStreamFiles) : base(allowed, blocked, streamFiles, doNotStreamFiles) { }

        public override void Process(PreprocessRequestArgs args)
        {
            string filePath;

            try
            {
                filePath = Path.GetFileName(HttpContext.Current.Request.FilePath);
            }
            catch (Exception e)
            {
                Log.Warn("Sitecore.Support.312141: handled invalid file path", e, this);
                return;
            }
            
            if (AllowedFileNames.Any() && AllowedFileNames.Contains(filePath ?? string.Empty))
            {
                return;
            }
            base.Process(args);
        }

        protected virtual IEnumerable<string> GetAllowedFileNames()
        {
            return Factory.GetConfigNodes("experienceAccelerator/sitecoreExtensions/filterUrlFilesAndExtensions/file").Cast<XmlNode>().Select(node => node.InnerText);
        }
    }
}