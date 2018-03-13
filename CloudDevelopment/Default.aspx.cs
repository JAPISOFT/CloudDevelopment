﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

public partial class _Default : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		MyLabel.Text = "Hello World from Azure & GitHub";
	}

	protected void GoButton_Click(object sender, EventArgs e)
	{
		if (MyFileUpload.HasFile)
		{
			var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
			var blobClient = storageAccount.CreateCloudBlobClient();
			var containerReference = blobClient.GetContainerReference("test");

			var blobReference = containerReference.GetBlockBlobReference(MyFileUpload.FileName);
			blobReference.UploadFromStream(MyFileUpload.FileContent);
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
		var blobClient = storageAccount.CreateCloudBlobClient();
		var containerReference = blobClient.GetContainerReference("test");

		var blobs = containerReference.ListBlobs();
		FilesRepeater.DataSource = blobs;
		FilesRepeater.DataBind();
	}

	protected void FileLink_Command(object sender, CommandEventArgs e)
	{
		var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
		var blobClient = storageAccount.CreateCloudBlobClient();

		var blobReference = new CloudBlockBlob(new Uri((string)e.CommandArgument), blobClient);
		blobReference.DownloadToStream(Response.OutputStream);
		// Just a demo - TODO: HTTP Response fine-tuning
	}
}