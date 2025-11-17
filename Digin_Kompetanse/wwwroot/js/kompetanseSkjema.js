// KOMPETANSEREGISTRERING (bedrift-skjema)
function initKompetanseForm() {
    const container = document.getElementById("kompetanse-container");
    const addRowBtn = document.getElementById("add-row");
    if (!container || !addRowBtn) return;

    const getKompetanserUrl      = window.getKompetanserUrl      || "/Home/GetKompetanser";
    const getUnderkompetanserUrl = window.getUnderkompetanserUrl || "/Home/GetUnderkompetanser";

    let rowIndex = document.querySelectorAll(".kompetanse-row").length || 0;

    // Legg til rad
    addRowBtn.addEventListener("click", () => {
        const firstRow = container.querySelector(".kompetanse-row");
        if (!firstRow) return;

        const clone = firstRow.cloneNode(true);
        clone.setAttribute("data-index", rowIndex);

        clone.querySelectorAll("select, textarea").forEach(el => {
            const name = el.getAttribute("name");
            const id   = el.getAttribute("id");

            if (name) el.name = name.replace(/\[\d+\]/, `[${rowIndex}]`);
            if (id)   el.id   = id.replace(/_\d+__/, `_${rowIndex}__`);

            if (el.tagName === "SELECT") {
                el.selectedIndex = 0;
                if (el.classList.contains("js-kompetanse")) {
                    el.innerHTML = '<option value="">Velg et fagområde først.</option>';
                    el.disabled = true;
                }
            } else if (el.tagName === "TEXTAREA") {
                el.value = "";
            }
        });

        clone.querySelectorAll(".kompetanse-group,.underkompetanse-group")
            .forEach(g => g.classList.add("d-none"));

        const underContainer = clone.querySelector(".js-underkompetanse-container");
        if (underContainer) {
            underContainer.innerHTML = '<p class="text-muted mb-0">Velg kompetanse først.</p>';
        }

        clone.querySelectorAll("span[data-valmsg-for]").forEach(span => span.textContent = "");

        container.appendChild(clone);
        rowIndex++;
    });

    // Fjern rad
    document.addEventListener("click", e => {
        if (!e.target.matches(".js-remove-row")) return;
        const rows = container.querySelectorAll(".kompetanse-row");
        if (rows.length > 1) e.target.closest(".kompetanse-row").remove();
    });

    // Fagområde → kompetanse
    document.addEventListener("change", async e => {
        if (!e.target.matches(".js-fagomrade")) return;

        const row  = e.target.closest(".kompetanse-row");
        const fagId = e.target.value;

        const kompGroup      = row.querySelector(".kompetanse-group");
        const kompSelect     = row.querySelector(".js-kompetanse");
        const underGroup     = row.querySelector(".underkompetanse-group");
        const underContainer = row.querySelector(".js-underkompetanse-container");

        if (kompSelect) {
            kompSelect.innerHTML = '<option value="">Velg et fagområde først.</option>';
            kompSelect.disabled = true;
        }
        if (kompGroup) kompGroup.classList.add("d-none");

        if (underGroup && underContainer) {
            underGroup.classList.add("d-none");
            underContainer.innerHTML = '<p class="text-muted mb-0">Velg kompetanse først.</p>';
        }

        if (!fagId) return;

        try {
            const res = await fetch(`${getKompetanserUrl}?fagområdeId=${encodeURIComponent(fagId)}`);
            if (!res.ok) throw new Error("Kunne ikke hente kompetanser");

            const data = await res.json();
            kompSelect.innerHTML = '<option value="">Velg kompetanse</option>';
            data.forEach(k => {
                const opt = document.createElement("option");
                opt.value = k.kompetanseId;
                opt.textContent = k.kompetanseKategori;
                kompSelect.appendChild(opt);
            });

            kompSelect.disabled = false;
            if (kompGroup) kompGroup.classList.remove("d-none");
        } catch (err) {
            alert("Kunne ikke hente kompetanser.");
        }
    });

    // Kompetanse → underkompetanse (chips)
    document.addEventListener("change", async e => {
        if (!e.target.matches(".js-kompetanse")) return;

        const row      = e.target.closest(".kompetanse-row");
        const rowIdx   = row.getAttribute("data-index");
        const kompId   = e.target.value;
        const underGroup     = row.querySelector(".underkompetanse-group");
        const underContainer = row.querySelector(".js-underkompetanse-container");

        underContainer.innerHTML = "";

        if (!kompId) {
            underGroup.classList.add("d-none");
            underContainer.innerHTML = '<p class="text-muted mb-0">Velg kompetanse først.</p>';
            return;
        }

        try {
            const res = await fetch(`${getUnderkompetanserUrl}?kompetanseId=${encodeURIComponent(kompId)}`);
            if (!res.ok) throw new Error("Kunne ikke hente underkompetanser");

            const data = await res.json();

            if (!data.length) {
                underContainer.innerHTML = '<p class="text-muted mb-0">Ingen underkompetanser registrert.</p>';
            } else {
                underContainer.innerHTML = data.map(u => {
                    const inputId = `Rader_${rowIdx}__UnderkompetanseId_${u.underkompetanseId}`;
                    const name    = `Rader[${rowIdx}].UnderkompetanseId`;

                    return `
                        <div class="tag-check">
                            <input type="checkbox"
                                   class="tag-check-input"
                                   name="${name}"
                                   value="${u.underkompetanseId}"
                                   id="${inputId}">
                            <label for="${inputId}" class="tag-check-label">
                                ${u.underkompetanseNavn}
                            </label>
                        </div>`;
                }).join("");
            }

            underGroup.classList.remove("d-none");
        } catch (err) {
            alert("Kunne ikke hente underkompetanser.");
            underGroup.classList.add("d-none");
        }
    });
}
