function updateTimera() {
  const future = Date.parse("April 23, 2027 11:30:00");
  const now = new Date();
  const diff = future - now;

  const days = Math.floor(diff / (1000 * 60 * 60 * 24));
  const hours = Math.floor(diff / (1000 * 60 * 60));
  const mins = Math.floor(diff / (1000 * 60));
  const secs = Math.floor(diff / 1000);

  const d = days;
  const h = hours - days * 24;
  const m = mins - hours * 60;
  const s = secs - mins * 60;

  const dealend = document.getElementById("dealend");
  if (dealend) {
      dealend.innerHTML =
          `<div class="dealend-timer">
              <div class="time-block"><div class="time">${d}</div><span class="day">Days</span></div>
              <div class="time-block"><div class="time">${h}</div><span class="dots">:</span></div>
              <div class="time-block"><div class="time">${m}</div><span class="dots">:</span></div>
              <div class="time-block"><div class="time">${s}</div><span class="dots"></span></div>
          </div>`;
  }
}
setInterval(updateTimera, 1000);

function updateGenericTimer(selector) {
  const timers = document.querySelectorAll(selector);
  if (timers.length === 0) return;

  const dateAttr = timers[0].getAttribute("data-date");
  if (!dateAttr) return;

  const futuretime = Date.parse(dateAttr);
  const now = new Date();
  const diff = futuretime - now;

  const days = Math.floor(diff / (1000 * 60 * 60 * 24));
  const hours = Math.floor(diff / (1000 * 60 * 60));
  const mins = Math.floor(diff / (1000 * 60));
  const secs = Math.floor(diff / 1000);

  const d = days;
  const h = hours - days * 24;
  const m = mins - hours * 60;
  const s = secs - mins * 60;

  timers.forEach(e => {
      e.innerHTML =
          `<div class="dealend-timer">
              <div class="time-block"><div class="time">${d}</div><span class="day">Days</span></div>
              <div class="time-block"><div class="time">${h}</div><span class="dots">:</span></div>
              <div class="time-block"><div class="time">${m}</div><span class="dots">:</span></div>
              <div class="time-block"><div class="time">${s}</div></div>
          </div>`;
  });
}

setInterval(() => updateGenericTimer('.timer1'), 1000);
setInterval(() => updateGenericTimer('.timer2'), 1000);
setInterval(() => updateGenericTimer('.timer3'), 1000);
