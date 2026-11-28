(function(){
  var ICONS={success:'\u2713',error:'\u2715',warning:'!',info:'i'};
  function box(){var b=document.querySelector('.ff-toasts');if(!b){b=document.createElement('div');b.className='ff-toasts';document.body.appendChild(b);}return b;}
  function esc(s){var d=document.createElement('div');d.textContent=s==null?'':s;return d.innerHTML;}
  window.ffToast=function(type,title,msg,ms){
    ms=ms||5000;var t=document.createElement('div');t.className='ff-toast '+type;t.style.setProperty('--d',ms+'ms');
    t.innerHTML='<div class="ic">'+(ICONS[type]||'i')+'</div><div><b>'+esc(title)+'</b>'+(msg?'<span>'+esc(msg)+'</span>':'')+'</div><button class="x" aria-label="Close">\u2715</button><i class="bar"></i>';
    box().appendChild(t);t.getBoundingClientRect();setTimeout(function(){t.classList.add('show');},30);
    var done=false;function close(){if(done)return;done=true;t.classList.add('hide');setTimeout(function(){t.remove();},400);}
    t.querySelector('.x').onclick=close;setTimeout(close,ms);return t;
  };
  window.ffConfirm=function(o){return new Promise(function(res){
    var ov=document.createElement('div');ov.className='ff-ov';
    ov.innerHTML='<div class="ff-modal" role="dialog" aria-modal="true"><div class="big"><i class="bi bi-trash3"></i></div><h3>'+esc(o.title||'Are you sure?')+'</h3><p>'+(o.html||esc(o.message||'This action cannot be undone.'))+'</p><div class="row"><button class="c">'+esc(o.cancel||'Cancel')+'</button><button class="d">'+esc(o.ok||'Delete')+'</button></div></div>';
    document.body.appendChild(ov);ov.getBoundingClientRect();setTimeout(function(){ov.classList.add('show');},20);
    function end(v){document.removeEventListener('keydown',k);ov.classList.remove('show');setTimeout(function(){ov.remove();},250);res(v);}
    function k(e){if(e.key==='Escape')end(false);}document.addEventListener('keydown',k);
    ov.querySelector('.c').onclick=function(){end(false);};ov.querySelector('.d').onclick=function(){end(true);};
    ov.addEventListener('mousedown',function(e){if(e.target===ov)end(false);});ov.querySelector('.d').focus();
  });};
  /* Big deletes: <form data-ff-confirm="Delete IT?" data-ff-message="..."> */
  document.addEventListener('submit',function(e){
    var f=e.target;if(!f.matches||!f.matches('form[data-ff-confirm]')||f.dataset.ffDone)return;
    e.preventDefault();
    ffConfirm({title:f.dataset.ffConfirm,message:f.dataset.ffMessage,ok:f.getAttribute('data-ff-ok')}).then(function(ok){if(ok){f.dataset.ffDone='1';f.submit();}});
  },true);
  /* Small deletes: <form data-ff-inline> with a submit button; first click arms, then Yes/No */
  document.addEventListener('click',function(e){
    var b=e.target.closest&&e.target.closest('form[data-ff-inline] [type=submit]');
    if(!b)return;var f=b.form;if(f.dataset.ffArmed)return;
    e.preventDefault();var w=document.createElement('span');w.className='ff-inline armed';
    f.parentNode.insertBefore(w,f);w.appendChild(f);
    b.classList.add('ff-trig');
    var ask=document.createElement('span');ask.className='ff-ask';
    ask.innerHTML='Sure? <button type="button" class="y">Yes</button><button type="button" class="n">No</button>';
    w.appendChild(ask);f.dataset.ffArmed='1';
    function reset(){w.classList.remove('armed');ask.remove();delete f.dataset.ffArmed;}
    ask.querySelector('.y').onclick=function(){f.dataset.ffArmed='';f.submit();};
    ask.querySelector('.n').onclick=reset;setTimeout(function(){if(f.dataset.ffArmed)reset();},4000);
  },true);
  /* TempData bridge: <div data-ff-toast="success|error|warning|info" data-title=".." data-msg=".."> */
  document.addEventListener('DOMContentLoaded',function(){
    document.querySelectorAll('[data-ff-toast]').forEach(function(n,i){
      setTimeout(function(){ffToast(n.dataset.ffToast,n.dataset.title||n.textContent.trim(),n.dataset.msg||'');},250+i*180);n.style.display='none';});
  });
})();
