using System;
using System.Collections.Generic;

namespace FUNewsManagementSystem.BusinessObjects.Entities;

public partial class SystemAccount
{
    public short AccountID { get; set; }

    public string? AccountName { get; set; }

    public string? AccountEmail { get; set; }

    public int? AccountRole { get; set; }

    public string? AccountPassword { get; set; }

    public string? GoogleId { get; set; }

    public string? AvatarUrl { get; set; }

    public bool IsExternalLogin { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}

