using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using LinearAlgebra;

namespace PoseCompressor
{
	public enum JointType
	{
		SpineBase,
		SpineMid,
		Neck,
		Head,
		ShoulderLeft,
		ElbowLeft,
		WristLeft,
		HandLeft,
		ShoulderRight,
		ElbowRight,
		WristRight,
		HandRight,
		HipLeft,
		KneeLeft,
		AnkleLeft,
		FootLeft,
		HipRight,
		KneeRight,
		AnkleRight,
		FootRight,
		SpineShoulder,
		HandTipLeft,
		ThumbLeft,
		HandTipRight,
		ThumbRight
	}

	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// a bone defined as a line between two joints
			bones = new List<Tuple<JointType, JointType>>();

			// Torso
			bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
			bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
			bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
			bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
			bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
			bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
			bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
			bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

			// Right Arm
			bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
			bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
			bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
			bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
			bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

			// Left Arm
			bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
			bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
			bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
			bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
			bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

			// Right Leg
			bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
			bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
			bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

			// Left Leg
			bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
			bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
			bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));
		}

		public string path = null;

		public XmlDocument originArchive = null;

		public XmlDocument compressedArchive = null;

		public XmlNodeList originBoneNodes = null;

		private List<Tuple<JointType, JointType>> bones;

		public string[] fileName;

		private void selectButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new Microsoft.Win32.OpenFileDialog();
			openFileDialog.Filter = "Kinect Pose Archives|KinectPoseArchive_????-??-??_??.??.??.xml";
			openFileDialog.DefaultExt = ".xml";
			openFileDialog.Multiselect = false;
			if (Directory.Exists(@"D:\Personal Files\OneDrive - bupt.edu.cn\Kinect Docs\Archives"))
			{
				openFileDialog.InitialDirectory = @"D:\Personal Files\OneDrive - bupt.edu.cn\Kinect Docs\Archives";
			}

			if ((bool)openFileDialog.ShowDialog())
			{
				try
				{
					path = openFileDialog.FileName;
					originArchive = new XmlDocument();
					originArchive.Load(path);
					originBoneNodes = originArchive.DocumentElement.ChildNodes;
				}
				catch
				{
					MessageBox.Show("存档格式有误，请重新选择！");
					return;
				}
				if (originBoneNodes[0].HasChildNodes)
				{
					fileName = openFileDialog.SafeFileName.Split('_', '-', '.');
					fileNameBlock.Text = $"{fileName[1]}/{fileName[2]}/{fileName[3]} {fileName[4]}:{fileName[5]}:{fileName[6]} 存档";
					cancelButton.Visibility = Visibility.Visible;
					startButton.IsEnabled = true;
					sizeBlock.Text = $"{new FileInfo(path).Length / 1024.0:0.00} KB";
					timeSpanBlock.Text = $"{Convert.ToDouble(originBoneNodes[23].LastChild.Attributes.GetNamedItem("Time").Value) / 1000.0:0.0} s";
					progressBlock.Text = "0 %";
					progressBar.Value = 0.0;

					selectButton.Visibility = Visibility.Hidden;
					selectButton.IsEnabled = false;
					fileName = null;
					fileName = openFileDialog.SafeFileName.Split('_');
				}
				else
				{
					MessageBox.Show("您选择的文件为空白录制。\n请重新选择！");
					path = null;
					originArchive = null;
					originBoneNodes = null;
				}
			}
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			cancelButton.Visibility = Visibility.Hidden;
			fileNameBlock.Text = "\0";
			selectButton.Visibility = Visibility.Visible;
			selectButton.IsEnabled = true;
			startButton.IsEnabled= false;
		}

		private void startButton_Click(object sender, RoutedEventArgs e)
		{
			startButton.Visibility = Visibility.Hidden;
			startButton.IsEnabled = false;
			compressingGrid.Visibility = Visibility.Visible;
			cancelButton.IsEnabled = false;
			compressedArchive = new XmlDocument();
			XmlElement rootElement = compressedArchive.CreateElement("CompressedPose");
			compressedArchive.AppendChild(rootElement);
			XmlElement[] compressedBoneNodes = new XmlElement[24];
			for (int i = 0; i < 24; i++)
			{
				compressedBoneNodes[i] = compressedArchive.CreateElement("Bone");
				compressedBoneNodes[i].SetAttribute("To", $"{bones[i].Item1}");
				compressedBoneNodes[i].SetAttribute("From", $"{bones[i].Item2}");
				rootElement.AppendChild(compressedBoneNodes[i]);
				Compress(originBoneNodes[i], ref compressedBoneNodes[i]);
				progressBar.Value = 100 * (i + 1) / 24.0;
				progressBlock.Text = $"{100 * (i + 1) / 24.0:0.0} %";
			}
			progressBlock.Text = "100 %";
			saveButton.Visibility = Visibility.Visible;
		}

		private void Compress(XmlNode originBoneNode, ref XmlElement compressedBoneNode)
		{
			//ColumnVector[] originData = new ColumnVector[originBoneNode.ChildNodes.Count];
			//for (int i = 0; i < originBoneNode.ChildNodes.Count; i++)
			//{
			//	originData[i] = ColumnVector.Parse(originBoneNode.ChildNodes[i].InnerText);
			//}
			List<ColumnVector> originData = new List<ColumnVector>();
			foreach(XmlNode node in originBoneNode.ChildNodes)
			{
				if (node.InnerText.Equals("null"))
				{
					originData.Add(null);
				}
				else
				{
					originData.Add(ColumnVector.Parse(node.InnerText));
				}
			}
			int ptr = 0;
			if (!originBoneNode.ChildNodes[0].Attributes.GetNamedItem("Time").Value.Equals("0"))
			{
				XmlElement newElement = compressedBoneNode.OwnerDocument.CreateElement("Node");
				int endTime = Convert.ToInt32(originBoneNode.ChildNodes[0].Attributes.GetNamedItem("Time").Value) - 1;
				newElement.SetAttribute("Begin", "0");
				newElement.SetAttribute("End", $"{endTime}");
				compressedBoneNode.AppendChild(newElement);
				//newElement.InnerText = "null";
			}
			while (ptr < originData.Count)
			{
				int i;
				for (i = ptr; i < originData.Count; i++)
				{
					if (originData[ptr] == null)
					{
						if (originData[i] != null)
						{
							break;
						}
						continue;
					}
					else if(originData[i] == null)
					{
						break;
					}
					else if (originData[ptr].Angle(originData[i]) > 0.1)
					{
						break;
					}
				}
				XmlElement newElement = compressedBoneNode.OwnerDocument.CreateElement("Node");
				int beginTime = Convert.ToInt32(originBoneNode.ChildNodes[ptr].Attributes.GetNamedItem("Time").Value);
				int endTime;
				if (originBoneNode.ChildNodes.Count == i)
				{
					endTime = Convert.ToInt32(originBoneNode.ChildNodes[i - 1].Attributes.GetNamedItem("Time").Value);
				}
				else
				{
					endTime = Convert.ToInt32(originBoneNode.ChildNodes[i].Attributes.GetNamedItem("Time").Value) - 1;
				}
				newElement.SetAttribute("Begin", $"{beginTime}");
				newElement.SetAttribute("End", $"{endTime}");
				if (originData[ptr] == null)
				{
					//newElement.InnerText = "null";
				}
				else
				{
					ColumnVector ans = new ColumnVector(3);
					for (int j = ptr; j < i; j++)
					{
						ans += originData[j] / 100.0;
					}
					ans = ans.Normalize() * 100.0;
					ans[1] = Math.Round(ans[1]);
					ans[2] = Math.Round(ans[2]);
					ans[3] = Math.Round(ans[3]);
					newElement.InnerText = ans.ToString();
				}
				compressedBoneNode.AppendChild(newElement);
				ptr = i;
			}
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			compressedArchive.Save($@"D:\Personal Files\OneDrive - bupt.edu.cn\Kinect Docs\Archives\CompressedPoseArchive_{fileName[1]}_{fileName[2]}");
			MessageBox.Show("压缩存档已保存。");
			compressingGrid.Visibility = Visibility.Hidden;
			saveButton.Visibility = Visibility.Hidden;
			startButton.Visibility = Visibility.Visible;
			cancelButton.Visibility = Visibility.Hidden;
			cancelButton.IsEnabled = true;
			selectButton.Visibility = Visibility.Visible;
			selectButton.IsEnabled = true;
			fileNameBlock.Text = "\0";
		}
	}
}
