# How to resize GaugeControl on printing/exporting


<p>This example illustrates an approach of including a <strong>GaugeControl</strong> image into a document generated via <a href="https://documentation.devexpress.com/#WPF/clsDevExpressXpfPrintingSimpleLinktopic">SimpleLink</a> (DXPrinting).</p>
<p>In the example, a gauge's image is obtained via the <strong>RenderTargetBitmap.Render</strong> method. Before rendering, special code is executed to resize the control based on the target documents requirements. </p>

<br/>


