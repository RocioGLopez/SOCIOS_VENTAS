document.addEventListener('DOMContentLoaded', function(){
  var toggle = document.querySelector('[data-toggle="contrasena"]');
  if(toggle){
    toggle.addEventListener('change', function(e){
      var pw = document.getElementById('contrasena');
      if(!pw) return;
      pw.type = e.target.checked ? 'text' : 'password';
    });
  }

  var form = document.getElementById('loginForm');
  if(form){
    form.addEventListener('submit', function(e){
      // pequeña animación y bloqueo del botón para evitar envíos repetidos
      var btn = form.querySelector('button[type="submit"]');
      if(btn){
        btn.disabled = true;
        btn.textContent = 'Ingresando...';
      }
    });
  }
});
