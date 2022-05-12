using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using LinearAlgebra;
using System.Text.RegularExpressions;

namespace PoseComparator
{
	internal static class Program
	{
		public class BoneArchive
		{
			private class BoneFrame
			{
				public ColumnVector bone;
				public int beginTime, endTime;

				public BoneFrame(ColumnVector bone, int beginTime, int endTime)
				{
					this.bone = bone;
					this.beginTime = beginTime;
					this.endTime = endTime;
				}
			}

			private List<BoneFrame> boneFrames = new List<BoneFrame>();

			public ColumnVector this[int time]
			{
				get
				{
					foreach (var frame in boneFrames)
					{
						if (frame.beginTime <= time && time <= frame.endTime)
						{
							return frame.bone;
						}
					}
					return null;
				}
			}

			public BoneArchive(XmlNodeList boneNode)
			{
				foreach (XmlNode node in boneNode)
				{
					int beginTime = Convert.ToInt32(node.Attributes.GetNamedItem("Begin").Value);
					int endTime = Convert.ToInt32(node.Attributes.GetNamedItem("End").Value);
					ColumnVector vector = null;
					if (node.InnerText != string.Empty)
					{
						vector = ColumnVector.Parse(node.InnerText);
					}
					boneFrames.Add(new BoneFrame(vector, beginTime, endTime));
				}
			}
		}

		private static readonly string path = @"D:\Personal Files\OneDrive - bupt.edu.cn\Kinect Docs\Archives\";

		static int Main()
		{
			#region Data Segment
			List<FileInfo> originArchives = new List<FileInfo>();
			List<FileInfo> compressedArchives = new List<FileInfo>();
			XmlDocument originXML = new XmlDocument(), compressedXML = new XmlDocument();
			List<XmlNode> originBoneNodes = new List<XmlNode>();
			List<BoneArchive> compressedBoneArchives = new List<BoneArchive>();
			StringBuilder detailBuilder = new StringBuilder();
			StreamWriter resultFile;
			int origin, compressed;
			int countAll = 0, countStrictlySimilar = 0, countSimilar = 0, countModelUndefined = 0, countUserUndefined = 0, countUndefined = 0;
			double difference = 0.2;
			#endregion

			//Console.WindowWidth = 50;
			Console.WriteLine("\n\tPose Comparator by Xia Jinyi\n");

			#region Get all the existed files.
			foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
			{
				if (Regex.IsMatch(file.Name, "KinectPoseArchive_[0-9]{4}-[0-9]{2}-[0-9]{2}_[0-9]{2}.[0-9]{2}.[0-9]{2}.xml"))
				{
					originArchives.Add(file);
				}
				if (Regex.IsMatch(file.Name, "CompressedPoseArchive_[0-9]{4}-[0-9]{2}-[0-9]{2}_[0-9]{2}.[0-9]{2}.[0-9]{2}.xml"))
				{
					compressedArchives.Add(file);
				}
			}
			#endregion

			#region Select files.
			Console.WriteLine("Select an original archive to continue.");
			Console.WriteLine("┌────────────────────────────────────────────────┐");
			Console.WriteLine("│ Num\tDate\t\tTime\t\tSize\t │");
			for (int i = 0; i < originArchives.Count; i++)
			{
				FileInfo originArchive = originArchives[i];
				string[] name = originArchive.Name.Split('_', '-', '.');
				double size = originArchive.Length / 1024.0;
				Console.WriteLine($"│ {i + 1}.\t{name[1]}/{name[2]}/{name[3]}\t{name[4]}:{name[5]}:{name[6]}\t{size:#0} KB\t │");
			}
			Console.WriteLine("└────────────────────────────────────────────────┘");
			Console.WriteLine("Please enter the number:");
			while (true)
			{
				try
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					int selected = Convert.ToInt32(Console.ReadLine());
					Console.ResetColor();
					if (selected > 0 && selected <= originArchives.Count)
					{
						origin = selected - 1;
						originXML.Load(originArchives[origin].FullName);
						if (originXML.DocumentElement.FirstChild.HasChildNodes)
						{
							break;
						}
						else
						{
							Console.ResetColor();
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("The selected archive is a BLANK archive!");
						}
					}
					else
					{
						Console.ResetColor();
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Your input is OUT OF RANGE!");
					}
				}
				catch
				{
					Console.ResetColor();
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Format ERROR!");
				}
				Console.ResetColor();
				Console.WriteLine("Please check and enter again:");
			}
			Console.WriteLine();

			Console.WriteLine("Select a compressed archive to continue.");
			Console.WriteLine("┌────────────────────────────────────────────────┐");
			Console.WriteLine("│ Num\tDate\t\tTime\t\tSize\t │");
			for (int i = 0; i < compressedArchives.Count; i++)
			{
				FileInfo compressedArchive = compressedArchives[i];
				string[] name = compressedArchive.Name.Split('_', '-', '.');
				double size = compressedArchive.Length / 1024.0;
				Console.WriteLine($"│ {i + 1}.\t{name[1]}/{name[2]}/{name[3]}\t{name[4]}:{name[5]}:{name[6]}\t{size:#0} KB\t │");
			}
			Console.WriteLine("└────────────────────────────────────────────────┘");
			Console.WriteLine("Please enter the number:");
			while (true)
			{
				try
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					int selected = Convert.ToInt32(Console.ReadLine());
					Console.ResetColor();
					if (selected > 0 && selected <= compressedArchives.Count)
					{
						compressed = selected - 1;
						compressedXML.Load(compressedArchives[compressed].FullName);
						break;
					}
					else
					{
						Console.ResetColor();
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Your input is OUT OF RANGE!");
					}
				}
				catch
				{
					Console.ResetColor();
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Format ERROR!");
				}
				Console.ResetColor();
				Console.WriteLine("Please check and enter again:");
			}
			Console.WriteLine();
			#endregion

			#region Read and prepare files.
			foreach (XmlNode originBoneNode in originXML.DocumentElement.ChildNodes)
			{
				originBoneNodes.Add(originBoneNode);
			}
			foreach (XmlNode compressedBoneNode in compressedXML.DocumentElement.ChildNodes)
			{
				compressedBoneArchives.Add(new BoneArchive(compressedBoneNode.ChildNodes));
			}

			resultFile = new StreamWriter($"{path}ComparisonReport_{DateTime.Now:yyyy-MM-dd_HH.mm.ss}.md");
			resultFile.WriteLine("# Comparison Result\n");
			resultFile.WriteLine($"*{DateTime.Now:yyyy/MM/dd HH:mm:ss}*\n");

			resultFile.WriteLine("## Summary\n");
			resultFile.WriteLine($"**Original Archive's Name:** {originArchives[origin].Name}\n");
			resultFile.WriteLine($"**Original Archive's Duration:** {originXML.DocumentElement.LastChild.LastChild.Attributes.GetNamedItem("Time").Value} ms\n");
			resultFile.WriteLine($"**Compressed Archive's Name:** {compressedArchives[compressed].Name}\n");
			resultFile.WriteLine($"**Compressed Archive's Duration:** {compressedXML.DocumentElement.LastChild.LastChild.Attributes.GetNamedItem("End").Value} ms\n");
			#endregion

			#region Compare files.
			detailBuilder.AppendLine("## Details\n");
			for (int i = 0; i < 24; i++)
			{
				detailBuilder.AppendLine($"### From {originBoneNodes[i].Attributes.GetNamedItem("From").Value} To {originBoneNodes[i].Attributes.GetNamedItem("To").Value}\n");
				int currCountAll = 0, currCountModelUndefined = 0, currCountUserUndefined = 0, currCountUndefined = 0, currCountSimilar = 0, currCountStrictlySimilar = 0;
				StringBuilder subBuilder = new StringBuilder("|Time|User|Standard|Offset|Result|\n|---|---|---|---|---|\n");
				foreach (XmlNode node in originBoneNodes[i].ChildNodes)
				{
					countAll++;
					currCountAll++;
					int time = Convert.ToInt32(node.Attributes.GetNamedItem("Time").Value);
					ColumnVector standard = compressedBoneArchives[i][time];
					ColumnVector current = node.InnerText == "null" ? null : ColumnVector.Parse(node.InnerText);
					subBuilder.Append($"|{time}|{(current == null? "null" : current.ToString())}|{(standard == null ? "null" : standard.ToString())}|");
					if (standard == null || current == null)
					{
						countUndefined++;
						currCountUndefined++;
						subBuilder.Append("Undefined|");
						if (standard == null)
						{
							countModelUndefined++;
							currCountModelUndefined++;
						}
						if (current == null)
						{
							countUserUndefined++;
							currCountUserUndefined++;
						}
						if (standard == null && current == null)
						{
							countSimilar++;
							currCountSimilar++;
							subBuilder.AppendLine("Similar*|");
						}
						else
						{
							subBuilder.AppendLine("Undefined|");
						}
					}
					else
					{
						double offset = standard.Angle(current);
						subBuilder.Append($"{offset:#0.00}|");
						if (offset < difference || standard.Equals(current))
						{
							countSimilar++;
							currCountSimilar++;
							countStrictlySimilar++;
							currCountStrictlySimilar++;
							subBuilder.AppendLine("Similar|");
						}
						else
						{
							subBuilder.AppendLine("Dissimilar|");
						}
					}
				}
				detailBuilder.AppendLine($"**Similarity:** {100.0 * currCountSimilar / currCountAll:#0.00} %\n");
				detailBuilder.AppendLine($"**Similarity (Strict):** {100.0 * currCountStrictlySimilar / currCountAll:#0.00} %\n");
				detailBuilder.AppendLine($"**Undefined (All):** {100.0 * currCountUndefined / currCountAll:#0.00} %\n");
				detailBuilder.AppendLine($"**Undefined (Standard):** {100.0 * currCountModelUndefined / currCountAll:#0.00} %\n");
				detailBuilder.AppendLine($"**Undefined (User):** {100.0 * currCountUserUndefined / currCountAll:#0.00} %\n");
				detailBuilder.AppendLine(subBuilder.ToString());
			}
			#endregion

			#region Display results in the console.
			Console.WriteLine($"Similarity:");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"{100.0 * countSimilar / countAll:#0.0} %\n");
			Console.ResetColor();
			Console.WriteLine($"Similarity (Strict):");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"{100.0 * countStrictlySimilar / countAll:#0.0} %\n");
			Console.ResetColor();
			#endregion

			#region End the program.
			resultFile.WriteLine($"**Similarity:** {100.0 * countSimilar / countAll:#0.00} %\n");
			resultFile.WriteLine($"**Similarity (Strict):** {100.0 * countStrictlySimilar / countAll:#0.00} %\n");
			resultFile.WriteLine($"**Undefined (All):** {100.0 * countUndefined / countAll:#0.00} %\n");
			resultFile.WriteLine($"**Undefined (Standard):** {100.0 * countModelUndefined / countAll:#0.00} %\n");
			resultFile.WriteLine($"**Undefined (User):** {100.0 * countUserUndefined / countAll:#0.00} %\n");
			resultFile.WriteLine(detailBuilder.ToString());
			resultFile.WriteLine($@"## Explanations

- We use the original archive as user’s one and the compressed archive as the standard one.

- The unit of time is millisecond; the unit of offset is radian.

- We assume that if the offset is less than {difference:#0.0}, both of the poses are similar.

- The “null” in the tables means that the Kinect sensor failed to detect the specified bone at that time.

- If there is at least one “null” in a same row of the table, the content of “Offset” in that row will be “Undefined”.

- If the status of both the user and the standard is “null” in a specified row of the table, the content of “Result” in that row will be “Similar\*”, in which the ‘\*’ symbol is used to distinguish with the regular “Similar”.

- The overall similarity stands for the percentage of “Similar” and “Similar\*”, while the “Similarity (Strict)” stands for only the percentage of “Similar”.

---

*Generated by Pose Comparator*

*Program Author: Xia Jinyi*

*Copyright (C) 2021-{DateTime.Now:yyyy}*");
			resultFile.Flush();
			resultFile.Close();
			Console.Write("The detailed report has been generated and saved.\nPress any key to exit...");
			Console.ReadKey(true);
			Console.WriteLine("\n");
			return 0;
			#endregion
		}
	}
}
