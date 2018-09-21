﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ichikara.YoutubeComment
{
    [Serializable]
    public class YoutubeAPIProfile
    {
        public string kind;
        public object etag;
        public string nextPageToken;
        public string prevPageToken;
        public PageInfo pageInfo;
        public List<object> items;

        public YoutubeAPIProfile()
        {
            pageInfo = new PageInfo();
        }

    }
    [Serializable]
    public class PageInfo
    {
        public int totalResults;
        public int resultsPerPage;
    }
}