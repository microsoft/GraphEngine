var g_width = -1;
var g_height = -1;
const c_layoutchange_widththreshold = 768;
const c_layoutchange_heightthreshold = 680;

function WidthChanged(w) {
    var ret = false;
    if ((g_width - c_layoutchange_widththreshold) * (w - c_layoutchange_widththreshold) <= 0) {
        ret = true;
    }

    g_width = w;


    return ret;
}

function HeightChanged(h) {
    var ret = false;
    if ((g_height - c_layoutchange_heightthreshold) * (h - c_layoutchange_heightthreshold) <= 0) {
        ret = true;
    }

    g_height = h;

    return ret;
}

function BodyResize() {
    if (navigator.userAgent.match(/(iPhone|iPod|iPad)/))
        return;

    if (WidthChanged(window.innerWidth)) {
        document.getElementById("show-arrow").removeAttribute("style");
        document.getElementById("hide-arrow").removeAttribute("style");

        document.getElementById("sidebar-wrapper").removeAttribute("style");
        document.getElementById("doc-content").removeAttribute("style");
    }
    if (HeightChanged(window.innerHeight)) {
        if (window.innerHeight <= c_layoutchange_heightthreshold)
            document.getElementById("sidebar-wrapper").style.position = "relative";
        else
            document.getElementById("sidebar-wrapper").style.position = "fixed";
    }
}

function AdjustLayout() {
    if (document.documentElement.offsetHeight == document.documentElement.scrollHeight) {
        // without vertical scroll bar
        document.getElementsByClassName("nav-top")[0].style.paddingRight = "17px";
    }
    if (navigator.userAgent.match(/(iPhone|iPod|iPad)/))
    {
        if (document.getElementById("show-arrow")) {
            document.getElementById("show-arrow").style.visibility = "hidden";
        }
        if (document.getElementById("hide-arrow")) {
            document.getElementById("hide-arrow").style.visibility = "hidden";
        }
    }
}
