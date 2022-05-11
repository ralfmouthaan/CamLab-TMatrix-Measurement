# CamLab-TMatrix-Measurement

This program allows the transmission matrix of a multimode fibre or a scattering medium to be measured. This can be done using a Fourier basis, a Hadamard basis, a macropixel basis, or a random basis for the input modes. The output modes are characterised using off-axis interferometry. The SLM must have been aligned using CamLab-SLM-Manual-Alignment, and the camera must have been set up using CamLab-OffAxisCamera-Setup. The user can select the number of macropixels to be used, as well as the number of measurements to be taken. The overall hologram size can be altered in the code.

This program only performs the measurement. The input holograms and output fields are stored as text files. To subsequently calculate the transmission matrix CamLab-TMatrix-Calculation scripts can be used.

This program allows a random hologram or a binary Spade/Club/Heart/Diamond hologram to be displayed, and the corresponding output field to be recorded. This can be used for testing purposes.

The measurement routines implement the phase drift correction algorithms described in my [Applied Optics paper](https://doi.org/10.1364/AO.454679), "Robust Correction of Interferometer Phase Drift in Transmission Matrix Measurements".

TIP: If the program crashes because of a problem with the FFTW.NET library, then re-install it using NuGet.
