
Este proyecto tiene un Copy posterior a la compilación que copia JBC_Connect.dll al directorio de 'JBC_Connect Examples'.
El proyecto de 'JBC_Connect Examples' no se incorpora a esta solución porque tiene referencia a JBC_Connect.dll y se entrega tal cual al cliente.

My Project -> Compile -> botón 'Build Events...' -> Porst-build event command line:

Copy "$(TargetPath)" "$(ProjectDir)..\JBC_Connect Examples"