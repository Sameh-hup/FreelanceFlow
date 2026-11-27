// Blurred background shapes that drift with scroll
(function () {
  var bg = document.createElement('div');
  bg.className = 'ff-bg';
  bg.setAttribute('aria-hidden', 'true');
  bg.innerHTML = '<i></i><i></i><i></i><i></i>';
  document.body.prepend(bg);
  var shapes = bg.querySelectorAll('i'), speeds = [0.25, -0.18, 0.12, -0.3], ticking = false;
  function update() {
    var y = window.scrollY || 0;
    shapes.forEach(function (s, k) { s.style.transform = 'translate3d(0,' + (y * speeds[k]) + 'px,0)'; });
    ticking = false;
  }
  window.addEventListener('scroll', function () { if (!ticking) { ticking = true; requestAnimationFrame(update); } }, { passive: true });
  update();
})();
