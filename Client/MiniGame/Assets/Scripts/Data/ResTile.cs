using System;
using System.Security;
using Framework;
using System.Collections.Generic;
namespace GameData
{
	[ResCfg]
	public class ResTile
	{
		[ResCfgKey]
		public int id { get; private set; }
		public string path { get; private set; }
		public ResTile(SecurityElement node)
		{
			id = int.Parse(node.Attribute("id"));
			path = node.Attribute("path");
		}
	}
}