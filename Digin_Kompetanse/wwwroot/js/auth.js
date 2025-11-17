// BEDRIFT-INNLOGGING (OTP)
function initOtpLogin() {
    const stepEmail = document.getElementById("step-email");
    const stepOtp   = document.getElementById("step-otp");
    if (!stepEmail || !stepOtp) return;

    const REQUEST_URL = '/auth/request-otp';
    const VERIFY_URL  = '/auth/verify-otp';

    const el = {
        stepEmail:  document.getElementById('step-email'),
        stepOtp:    document.getElementById('step-otp'),
        email:      document.getElementById('email'),
        otp:        document.getElementById('otp'),
        btnSend:    document.getElementById('btnSend'),
        btnVerify:  document.getElementById('btnVerify'),
        btnBack:    document.getElementById('btnBack'),
        btnResend:  document.getElementById('btnResend'),
        message:    document.getElementById('message')
    };

    function setMsg(text, type = 'muted') {
        if (!el.message) return;
        el.message.innerHTML = `<span class="text-${type}">${text}</span>`;
    }

    function toggle(disabled) {
        [el.btnSend, el.btnVerify, el.btnResend, el.btnBack].forEach(b => b && (b.disabled = disabled));
        if (el.email) el.email.disabled = disabled;
        if (el.otp)   el.otp.disabled   = disabled;
    }

    async function postJson(url, payload) {
        const res = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        let data = {};
        try { data = await res.json(); } catch { /* ignorér */ }

        return { ok: res.ok, status: res.status, data };
    }

    // Steg 1: be om kode
    el.btnSend?.addEventListener('click', async () => {
        const email = el.email.value.trim();
        if (!email) {
            el.email.reportValidity();
            el.email.focus();
            return;
        }

        toggle(true);
        setMsg('Sender kode …');

        const { ok, data, status } = await postJson(REQUEST_URL, { email });
        toggle(false);

        if (ok) {
            setMsg(data.message ?? 'Vi har sendt en engangskode til e-posten din.', 'success');
            el.stepEmail.classList.add('d-none');
            el.stepOtp.classList.remove('d-none');
            el.otp.focus();
        } else {
            const msg = data.message || (status === 404 ? 'E-post ikke registrert.' : 'Kunne ikke sende kode.');
            setMsg(msg, 'danger');
        }
    });

    // Steg 2: verifiser kode
    el.btnVerify?.addEventListener('click', async () => {
        const email = el.email.value.trim();
        const code  = el.otp.value.trim();

        if (!code) {
            el.otp.reportValidity();
            el.otp.focus();
            return;
        }

        toggle(true);
        setMsg('Verifiserer kode …');

        const { ok, data, status } = await postJson(VERIFY_URL, { email, code });
        toggle(false);

        if (ok) {
            window.location.href = '/Home/Overview';
        } else {
            const msg = data.message || (status === 400 ? 'Feil eller utløpt kode.' : 'Noe gikk galt under verifisering.');
            setMsg(msg, 'danger');
        }
    });

    // Send ny kode i steg 2
    el.btnResend?.addEventListener('click', async () => {
        const email = el.email.value.trim();
        if (!email) {
            el.email.reportValidity();
            el.email.focus();
            return;
        }

        toggle(true);
        setMsg('Sender ny kode …');

        const { ok, data } = await postJson(REQUEST_URL, { email });
        toggle(false);

        setMsg(
            ok ? (data.message ?? 'Ny kode sendt.') : (data.message ?? 'Kunne ikke sende ny kode.'),
            ok ? 'success' : 'danger'
        );
    });

    // Tilbake til e-poststeg
    el.btnBack?.addEventListener('click', () => {
        el.stepOtp.classList.add('d-none');
        el.stepEmail.classList.remove('d-none');
        setMsg('');
        el.email.focus();
    });

    // Enter-handling
    el.email?.addEventListener('keydown', e => { if (e.key === 'Enter') el.btnSend.click(); });
    el.otp?.addEventListener('keydown',   e => { if (e.key === 'Enter') el.btnVerify.click(); });
}
