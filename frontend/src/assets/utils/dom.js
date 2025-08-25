export function getScrollbarWidth() {
  const scrollDiv = document.createElement("div");
  scrollDiv.style.visibility = "hidden";
  scrollDiv.style.overflow = "scroll";
  scrollDiv.style.msOverflowStyle = "scrollbar"; // Для IE 11
  scrollDiv.style.width = "50px";
  scrollDiv.style.height = "50px";

  document.body.appendChild(scrollDiv);

  const innerDiv = document.createElement("div");
  innerDiv.style.width = "100%";
  innerDiv.style.height = "100%";
  scrollDiv.appendChild(innerDiv);

  const scrollbarWidth = scrollDiv.offsetWidth - innerDiv.offsetWidth;

  document.body.removeChild(scrollDiv);

  return scrollbarWidth;
}
