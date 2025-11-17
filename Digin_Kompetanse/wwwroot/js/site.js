document.addEventListener("DOMContentLoaded", () => {
    if (typeof initAdminFilters === "function")      initAdminFilters();
    if (typeof initOtpLogin === "function")          initOtpLogin();
    if (typeof initKompetanseForm === "function")    initKompetanseForm();
    if (typeof initOverviewEditing === "function")   initOverviewEditing();
    if (typeof initLeggTilKompetanse === "function") initLeggTilKompetanse();
});
