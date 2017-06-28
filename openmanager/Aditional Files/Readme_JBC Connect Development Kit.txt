-----------------------
| JBC Connect Library |
-----------------------

Version: 3.15.1.4

NOTE: This is a test version of the library, it is posible to find some bugs.
NOTE2: NET Framework 4.0 is required

   1) In this compressed file you'll find the Dynamic Link Library (DLL) JBC_Connect.dll and two test projects of it.
      It also contains the required drivers for the JBC stations to PC USB connection.

   2) The projects are intended to be sample codes to understand and simplify JBC_Connect.dll usage.

   3) The main files are structured as follows:
      \root
	  \JBC Connect Examples
		  \JBC_Connect.dll ( The library )
		  \Tests
		  	\Tests.sln ( The VisualStudio 2010 solution with the two test prjects source code )
	  \Drivers
		  \CP210x_VCP_Win_XP_S2K3_Vista_7_8_81_x86.exe ( The required 32 bits USB driver for connecting JBC stations to PC )
		  \CP210x_VCP_Win_XP_S2K3_Vista_7_8_81_x64.exe ( The required 64 bits USB driver for connecting JBC stations to PC )
		  \CDC_driver_x86_x64.inf ( The required USB driver for the new Excellence JBC stations )
		  \dotNetFx40_Full_setup.exe ( The .NET Framework 4.0 installer, required this framework version )

   4)In order to compile the test projects you may require to reference the library (DLL) in the test projects at its 
     properties page.