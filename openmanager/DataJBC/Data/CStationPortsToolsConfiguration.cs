// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{
	public class CStationsConfiguration
	{
		private int m_Ports = 0;
		private eStationType m_StationType;
		private List<GenericStationTools> m_listSupportedTools = new List<GenericStationTools>();

		private enum StationPortsToolsConfiguration
		{
			// Representa: Nombre estaciÃ³n + NÃºmero de puertos + tipo de estaciÃ³n + Herramientas aceptadas
			// Todo ello separado por '_'
			// SOLD STATIONS (tipo = 1)
			PSE_4_1_T210_T245_PA_HT_DS_DR,
			DM_4_1_T210_T245_PA_HT_DS_DR,
			DME_4_1_T210_T245_PA_HT_DS_DR,
			DD_2_1_T210_T245_PA_HT_DS_DR,
			DDE_2_1_T210_T245_PA_HT_DS_DR,
			DDR_2_1_T210_T245_PA_HT_DS_DR,
			DI_1_1_T210_T245_PA_HT_DS_DR,
			HD_1_1_T245,
			HDE_1_1_T245,
			HDR_1_1_T245,
			CDCF_1_1_T210_T245, // viene como CD/CF
			CSCV_1_1_DS, // viene como CS/CV
			CP_1_1_PA,
			NA_2_1_NT105_NP105,
			NAE_2_1_NT105_NP105,
			SM_1_1_T210_T245,

			// AIR DESOLD STATIONS (tipo = 2)
			JT_1_2_JT_TE,
			JTSE_1_2_JT_TE, //_PHS_PHB
			// TIN FEEDER (tipo = 3)
			SF_1_3_NOTOOLS,
			
			// FUME EXTRACTOR (tipo = 4)
			FAE_2_4_NOTOOLS,

			UNDEFINED_0_
		}

		public CStationsConfiguration(string model)
		{
			BuildPortsTypeTools(model);
		}

		public GenericStationTools[] SupportedTools
		{
			get
			{
				return m_listSupportedTools.ToArray();
			}
		}

		public int Ports
		{
			get
			{
				return m_Ports;
			}
		}

		public eStationType StationType
		{
			get
			{
				return m_StationType;
			}
		}

		private void BuildPortsTypeTools(string model)
		{
			m_Ports = 0;
			m_StationType = eStationType.UNKNOWN;

			//Standarize model name
			model = model.Replace("/", "");

			//Buscamos la configuraciÃ³n para el modelo de estaciÃ³n
			foreach (string StationUnit in System.Enum.GetNames(typeof(StationPortsToolsConfiguration)))
			{

				// 31/01/2014 Buscar con guiÃ³n bajo ("DD_") porque varias pueden empezar igual y ser distintas
				if (StationUnit.ToString().IndexOf(model + "_") == 0)
				{
					string[] arrStationConf = StationUnit.ToString().Split('_');
					if (arrStationConf.Length >= 4)
					{
						// ports
						m_Ports = System.Convert.ToInt32(arrStationConf[1]);
						// station type
						m_StationType = (eStationType)((eStationType)(System.Convert.ToInt32(arrStationConf[2])));
						//supported tools
						for (var idxTool = 0; idxTool <= arrStationConf.Length - 4; idxTool++)
						{
							string sTool = arrStationConf[idxTool + 3];
							//Guardamos la tool
							if (System.Enum.IsDefined(typeof(GenericStationTools), sTool))
							{
								m_listSupportedTools.Add((GenericStationTools)(System.Enum.Parse(typeof(GenericStationTools), sTool)));
							}
						}
					}

					break;
				}
			}
		}

	}
}
