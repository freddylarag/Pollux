﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class Response
    {
        public Response()
        {
            Content = "";
            IsSuccessStatusCode = true;
            StatusCode = HttpStatusCode.OK;
        }

        public string Content { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public string Path { get; set; }

        public string RelativePath(string workspace)
        {
            string relative = Path.ToLower().Replace((System.IO.Path.GetDirectoryName(workspace)).ToLower(), "");
            if (relative.Substring(0, 1) == "\\")
            {
                relative = relative.Substring(1);
            }
            relative = relative.Replace("\\", "/");
            return relative;
        }
    }
}
