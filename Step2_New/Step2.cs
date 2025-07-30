using Kompas6API5;


using System;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Globalization;
using KAPITypes;
using Kompas6Constants;
using KompasLibrary;


namespace Steps.NET
{
	// ����� Step2 - ����������

	// 1. �������� ������                      - Intersect2Line
	// 2. �������� ������                      - Intersect2Curve
	// 3. �������� ������� � ����              - IntersectLineSegArc
	// 4. ����������� �� �����                 - TanLinePointCircle
	// 5. ����������� ��� �����                - TanLineAngCircle
	// 6. ������� �����                        - RotatePoint
	// 7. ��������� �����                      - SymmetryPoint
	// 8. ����������� ���������� � ���� ������ - Couplin2Lines
	// 9. ��������������                       - Perpendicular

	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class Step2New : KompasLibrary.IKompasLibrary
	{
		private KompasObject kompas;
		private ksDocument2D doc;
		private ksMathematic2D mat;

		// ��������� ����� ����������� � ��������� doc ��
		// ����������� ������� � ������ ������������ �� ���������
		private void DrawPointByArray(ksDynamicArray arr)
		{
			if (arr != null)
			{
				// ������� ��������� ���������� �������������� �����
				ksMathPointParam par = (ksMathPointParam)kompas.GetParamStruct((short)StructType2DEnum.ko_MathPointParam);

				if (par != null)
				{
					// ��������� ������
					for (int i = 0; i < arr.ksGetArrayCount(); i++)
					{
						arr.ksGetArrayItem(i, par);
						doc.ksPoint(par.x, par.y, 5);
						string buf = string.Format("x = {0:.##} y = {1:.##}", par.x, par.y);
						kompas.ksMessage(buf);
					}
				}
			}
		}


		// �������� ������
		private void IntersectLines()
		{
			ksDynamicArray arr = (ksDynamicArray)kompas.GetDynamicArray(ldefin2d.POINT_ARR);
			if (arr != null)
			{
				doc.ksLine(10, 10, 0);
				doc.ksLine(15, 5, 90);
				mat.ksIntersectLinLin(10, 10, 0, 15, 5, 90, arr);
				DrawPointByArray(arr);
				arr.ksDeleteArray();
			}
		}


		// �������� ������
		private void IntersectCurves()
		{
			ksDynamicArray arr = (ksDynamicArray)kompas.GetDynamicArray(ldefin2d.POINT_ARR);
			if (arr != null)
			{
				doc.ksBezier(0, 0);
				doc.ksPoint(20, 0, 0);
				doc.ksPoint(10, 20, 0);
				doc.ksPoint(20, 40, 0);
				doc.ksPoint(30, 20, 0);
				doc.ksPoint(20, 0, 0);
				int pp1 = doc.ksEndObj();

				doc.ksBezier(0, 0);
				doc.ksPoint(0, 20, 0);
				doc.ksPoint(20, 10, 0);
				doc.ksPoint(40, 20, 0);
				doc.ksPoint(20, 30, 0);
				doc.ksPoint(0, 20, 0);
				int pp2 = doc.ksEndObj();

				mat.ksIntersectCurvCurv(pp1, pp2, arr);
				DrawPointByArray(arr);
				arr.ksDeleteArray();
			}
		}


		// �������� ������� � ����
		private void IntercectLineSegAndArc()
		{
			ksDynamicArray arr = (ksDynamicArray)kompas.GetDynamicArray(ldefin2d.POINT_ARR);

			if (arr != null)
			{
				doc.ksLineSeg(0, 40, 100, 40, 1);         // ��������� �������
				doc.ksArcByPoint(50, 40, 20, 30, 40, 70, 40, 1, 1); // ��������� ���� �� ������ � �������� ������
				double a1 = mat.ksAngle(50, 40, 30, 40);      // ��������� ���� ����
				double a2 = mat.ksAngle(50, 40, 70, 40);      // �������� ���� ����

				// �������� ���������� ����� ����������� ������� � ����
				// ������ ����� ������� (0, 40), ������ ����� ������� (100, 40),
				// ����� ���� (50, 40), ������ ���� 20

				mat.ksIntersectLinSArc(0, 40, 100, 40, 50, 40, 20, a1, a2, 1, arr);
				DrawPointByArray(arr);  // ��������� ����� �����������

				arr.ksDeleteArray();
			}
		}


		// ����������� �� �����
		private void TanFromPoint()
		{
			ksDynamicArray arr = (ksDynamicArray)kompas.GetDynamicArray(ldefin2d.POINT_ARR);
			ksMathPointParam par = (ksMathPointParam)kompas.GetParamStruct((short)StructType2DEnum.ko_MathPointParam);

			if ((arr != null) & (par != null))
			{
				doc.ksPoint(10, 50, 3);     // ��������� �����
				doc.ksCircle(50, 10, 40, 1);  // ��������� ����������

				// �������� ����� ������� ���������� � ������, ���������� ����� �������� �����
				// ���������� ������� ����� (10, 50), ���������� ������ (50, 10),
				// ������ ���������� 40

				mat.ksTanLinePointCircle(10, 50, 50, 10, 40, arr);
				DrawPointByArray(arr);    // ��������� ����� �����������

				// ��������� �����������
				for (int i = 0; i < arr.ksGetArrayCount(); i++)
				{
					arr.ksGetArrayItem(i, par); // ��������� ������� �����
					doc.ksLine(10, 50, mat.ksAngle(10, 50, par.x, par.y));
				}

				arr.ksDeleteArray();
			}
		}


		// ����������� ��� �����
		private void TanToAngle()
		{
			ksDynamicArray arr = (ksDynamicArray)kompas.GetDynamicArray(ldefin2d.POINT_ARR);
			ksMathPointParam par = (ksMathPointParam)kompas.GetParamStruct((short)StructType2DEnum.ko_MathPointParam);

			if ((arr != null) & (par != null))
			{
				doc.ksLineSeg(0, 40, 100, 40, 1); // ��������� �������
				doc.ksCircle(50, 10, 40, 1);    // ��������� ����������

				// �������� ����� ������� ���������� � ������, ���������� ��� �������� �����
				// ���������� ������ (50, 10), ������ ���������� 40, ���� ����������� ������ 45

				mat.ksTanLineAngCircle(50, 10, 40, 45, arr);
				DrawPointByArray(arr);      // ��������� ����� �����������

				// ��������� �����������
				for (int i = 0; i < arr.ksGetArrayCount(); i++)
				{
					arr.ksGetArrayItem(i, par); // ��������� ������� �����
					doc.ksLine(par.x, par.y, 45);
				}

				arr.ksDeleteArray();
			}
		}


		// ������� �����
		private void RotatePoint()
		{
			double x = 0;
			double y = 0; //��������� ��������

			doc.ksPoint(60, 50, 3); //��������� �����
			doc.ksPoint(50, 50, 2); //��������� �����

			mat.ksRotate(60, 50, 50, 50, 180, out x, out y);  //������� �����

			doc.ksPoint(x, y, 5); // ��������� �������������� �����
														// ��������� ��������
			kompas.ksMessage(string.Format("x = {0:.##} y = {1:.##}", x, y));
		}


		//��������� �����
		private void SymmetryPoint()
		{
			double x = 0; // ��������� ��������� �����
			double y = 0;

			doc.ksPoint(30, 60, 3);       // ��������� �����
			doc.ksLineSeg(0, 50, 60, 50, 3);  // ��������� �������

			// �������� ���������� �����, ������������ ������������ �������� ���
			mat.ksSymmetry(30, 60, 0, 50, 60, 50, out x, out y);

			doc.ksPoint(x, y, 5); // ��������� �������������� �����
														// ��������� ���������
			kompas.ksMessage(string.Format("x = {0:.##} y = {1:.##}", x, y));
		}


		// ����������� ���������� � ���� ������
		private void CouplingTwoLines()
		{
			// ������� ��������� ������� ��������� ����� ����������
			ksCON iCON = (ksCON)kompas.GetParamStruct((short)StructType2DEnum.ko_CON);

			if (iCON != null) // ��������� ������
			{
				doc.ksLine(100, 100, 45); // ��������� ������ - ������ ������
				doc.ksLine(100, 100, -45);  // ������ ������

				// �������� ��������� �����������, ����������� � ���� ������
				// ������ ���������� 20

				mat.ksCouplingLineLine(100, 100, 45, 100, 100, -45, 20, iCON);

				// ��������� ������������� ����������� � ����� �������
				for (int i = 0; i < iCON.GetCount(); i++)
				{
					doc.ksCircle(iCON.GetXc(i), iCON.GetYc(i), 20, 2);
					doc.ksPoint(iCON.GetX1(i), iCON.GetY1(i), i);
					doc.ksPoint(iCON.GetX2(i), iCON.GetY2(i), i);
				}

				// ��������� ����������
				string msg = string.Format("count = {0:.##} con[0].x1 = {1:.##} con[0].y1 = {2:.##} con[0].x2 = {3:.##} con[0].y2 = {4:.##} ...",
					iCON.GetCount(),
					iCON.GetX1(0),
					iCON.GetY1(0),
					iCON.GetX2(0),
					iCON.GetY2(0));
				kompas.ksMessage(msg);
			}
		}


		// ��������������
		private void BuildPerpend()
		{
			doc.ksPoint(50, 50, 2);       // ��������� �����
			doc.ksLineSeg(60, 10, 100, 10, 1);  // ��������� �������

			double x = 0;   // ����� ����������� ������� � ��������������
			double y = 0;

			// ���������� ����� ����������� ������� � ��������������
			// ���������� ������������ ������� ����� (50, 50)
			// ���������� ������ ����� ������� (60, 10), ���������� ������ ����� ������� (100, 10)

			mat.ksPerpendicular(50, 50, 60, 10, 100, 10, out x, out y);
			// ��������� ��������������
			doc.ksLine(50, 50, mat.ksAngle(50, 50, x, y));
			// ��������� ����� ����������� �������
			doc.ksPoint(x, y, 5);

			// ��������� ������� ��������������
			kompas.ksMessage(string.Format("x = {0:.##} y = {1:.##}", x, y));
		}

		public bool get_IsFunctionEnable(ksKompasLibraryFunctionEnum FunctionID)
		{
      //    ksLFVersion = 17001,
      //    ksLFIsFunctionEnable = 17002,
      //    ksLFLibraryName = 17003,
      //    ksLFDisplayLibraryName = 17004,
      //    ksLFLibraryHelpFile = 17005,
      //    ksLFProtectNumber = 17006,
      //    ksLFRunLibraryCommand = 17007,
      //    ksLFIsOnApplication7 = 17008,
      //    ksLFInitLibrary = 17009,
      //    ksLFBeginUnloadLibrary = 17010,
      //    ksLFFillLibraryMenu = 17011,
      //    ksLFGetLibraryCommandState = 17012,
      //    ksLFGetDisableReason = 17013,
      //    ksLFFillContextPanel = 17014,
      //    ksLFContextPanelStyleComboChanged = 17015,
      //    ksLFGetKompasConverter = 17016,
      //    ksLFCreateMacroFromSample = 17017


      return FunctionID == ksKompasLibraryFunctionEnum.ksLFLibraryName ||
						 FunctionID == ksKompasLibraryFunctionEnum.ksLFRunLibraryCommand ||
						 FunctionID == ksKompasLibraryFunctionEnum.ksLFInitLibrary ||
						 FunctionID == ksKompasLibraryFunctionEnum.ksLFBeginUnloadLibrary ||
						 FunctionID == ksKompasLibraryFunctionEnum.ksLFFillLibraryMenu ||
						 FunctionID == ksKompasLibraryFunctionEnum.ksLFGetLibraryCommandState ||
						 FunctionID == ksKompasLibraryFunctionEnum.ksLFGetDisableReason;

		}
		public bool InitLibrary([In, MarshalAs(UnmanagedType.IDispatch)] object ApplicationInterface)
		{
			kompas = (KompasObject)ApplicationInterface;
			return true;
		}
		public bool BeginUnloadLibrary()
		{
			kompas = null;
			return true;
		}

		public bool FillLibraryMenu(IKompasLibraryMenu Menu)
		{
			if (Menu == null)
				return false;
      Menu.AddSubMenu("��������");
			{
				Menu.AddMenuCommand(1, "�������� ������");
				Menu.AddMenuCommand(2, "�������� ������");
				Menu.AddMenuCommand(3, "�������� ������� � ����");
				Menu.EndSubMenu();
			}
			Menu.AddMenuCommand(4, "����������� �� �����");
			Menu.AddMenuCommand(5, "����������� ��� �����");
			Menu.AddMenuCommand(6, "������� �����");
			Menu.AddMenuCommand(7, "��������� �����");
			Menu.AddMenuCommand(8, "����������� ���������� � ���� ������");
			Menu.AddMenuCommand(9, "�������������");
      return true;
		}

		public bool GetLibraryCommandState(int Command, out bool Enable, out int Checked)
		{

			ksDocument2D doc2D = (ksDocument2D)kompas.ActiveDocument2D();
			if (doc2D == null)
				Enable = false;
			else
				Enable = true;
			Checked = 0;
			return true;
		}
		[return: MarshalAs(UnmanagedType.BStr)] public string GetDisableReason(int Command)
		{
			string res = string.Empty; 
			ksDocument2D doc2D = (ksDocument2D)kompas.ActiveDocument2D();
			if (doc2D == null)
			{
				res = "����������: ��� ��������� 2D ���������";
			}
			return res;
		}
		public bool FillContextPanel(object ContextPanel)
		{
			return false;
    }
    public bool ContextPanelStyleComboChanged(string StyleComboID, int styleType, int newValue)
		{
			return false;
		}

		public dynamic GetKompasConverter()
		{
			return null;
		}
		public bool CreateMacroFromSample(int MacroReference)
		{
			return false;
		}

		public int Version { get { return 1; } }
		[return: MarshalAs(UnmanagedType.BStr)]  public string LibraryName { get { return "Step2 - ������������e �������������� �������";  } }
		[return: MarshalAs(UnmanagedType.BStr)]  public string DisplayLibraryName { get { return LibraryName; } }
		public string LibraryHelpFile { get { return ""; } }
		public int ProtectNumber { get { return 0;  } }
		public bool IsOnApplication7 { get { return false; } }

//    [return: MarshalAs(UnmanagedType.BStr)] public string GetLibraryName()
//		{
//			return "Step2 - ������������e �������������� �������";
//		}
//
//
//		[return: MarshalAs(UnmanagedType.BStr)] public string ExternalMenuItem(short number, ref short itemType, ref short command)
//		{
//			string result = string.Empty;
//			return result;
//		}

	
		//public void ExternalRunCommand([In] short command, [In] short mode, [In, MarshalAs(UnmanagedType.IDispatch)] object kompas_)
    public int RunLibraryCommand(int Command, int DemoMode)
		{
//			kompas = (KompasObject) kompas_;

			if (kompas == null)
				return 0;

			doc = (ksDocument2D) kompas.ActiveDocument2D();

			if (doc == null)
				return 0;

			mat = (ksMathematic2D) kompas.GetMathematic2D();

			if (mat == null)
				return 0;

			switch (Command)
			{
				case 1:	IntersectLines();			break; // �������� ������
				case 2: IntersectCurves();			break; // �������� ������
				case 3: IntercectLineSegAndArc();	break; // �������� ������� � ����
				case 4:	TanFromPoint();				break; // ����������� �� �����
				case 5:	TanToAngle();				break; // ����������� ��� �����
				case 6:	RotatePoint();				break; // ������� �����
				case 7:	SymmetryPoint();			break; // ��������� �����
				case 8:	CouplingTwoLines();			break; // ����������� ���������� � ���� ������
				case 9:	BuildPerpend();				break; // ��������������
			}

			kompas.ksMessageBoxResult();
			return 1;
		}


		public object ExternalGetResourceModule()
		{
			return Assembly.GetExecutingAssembly().Location;
		}


		// public int ExternalGetToolBarId(short barType, short index)
		// {
		// 	int result = 0;
		// 
		// 	if (barType == 0)
		// 	{
		// 		result = -1;
		// 	}
		// 	else
		// 	{
		// 		switch (index)
		// 		{
		// 			case 1:
		// 				result = 3001;
		// 				break;
		// 			case 2:
		// 				result = -1;
		// 				break;
		// 		}
		// 	}
		// 
		// 	return result;
		// }


		#region COM Registration
		// ��� ������� ����������� ��� ����������� ������ ��� COM
		// ��� ��������� � ����� ������� ���������� ������ Kompas_Library,
		// ������� ������������� � ���, ��� ����� �������� ����������� ������,
		// � ����� �������� ��� InprocServer32 �� ������, � ��������� ����.
		// ��� ��� �������� ��� ����, ����� ����� ����������� ����������
		// ���������� �� ������� ActiveX.
		[ComRegisterFunction]
		public static void RegisterKompasLib(Type t)
		{
			try
			{
				RegistryKey regKey = Registry.LocalMachine;
				string keyName = @"SOFTWARE\Classes\CLSID\{" + t.GUID.ToString() + "}";
				regKey = regKey.OpenSubKey(keyName, true);
				regKey.CreateSubKey("Kompas_Library");
				regKey = regKey.OpenSubKey("InprocServer32", true);
				regKey.SetValue(null, System.Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\mscoree.dll");
				regKey.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("��� ����������� ������ ��� COM-Interop ��������� ������:\n{0}", ex));
			}
		}
		
		// ��� ������� ������� ������ Kompas_Library �� �������
		[ComUnregisterFunction]
		public static void UnregisterKompasLib(Type t)
		{
			RegistryKey regKey = Registry.LocalMachine;
			string keyName = @"SOFTWARE\Classes\CLSID\{" + t.GUID.ToString() + "}";
			RegistryKey subKey = regKey.OpenSubKey(keyName, true);
			subKey.DeleteSubKey("Kompas_Library");
			subKey.Close();
		}
		#endregion
	}

}
