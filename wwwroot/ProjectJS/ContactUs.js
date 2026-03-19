var _Id = 0;
var existingImageNames = [];
var existingEmails = [];
var highResPreview = '';
let generatedOTP = "";
let emailVerified = false;
let emailSent = false;
let lastVerifiedEmail = "";
let otpTimer;
let otpTimeLeft = 0;
function isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function ValidateImage() {
    var file = $('#imageFile')[0].files[0];
    if (_Id === 0 && !file) {
        toastr.error("Image is required");
        return false;
    }
    if (file) {
        var allowedTypes = ["image/jpeg", "image/png", "image/jpg", "image/webp", "image/avif", "image/gif", "image/svg+xml", "image/bmp", "image/tiff"];
        if (!allowedTypes.includes(file.type)) {
            toastr.error("Only JPG, PNG, GIF, BMP, WEBP, SVG, AVIF, TIFF allowed");
            return false;
        }
        if (file.size > 2 * 1024 * 1024) {
            toastr.error("Image size must be less than 2MB");
            return false;
        }
    }
    return true;
}


function ValidateDuplicateImage(file) {
    if (!file) return true;

    let fileName = file.name.toLowerCase().trim();

    // New record
    if (_Id === 0) {
        if (existingImageNames.some(img => img.name === fileName)) {
            toastr.error("This image already exists!");
            return false; // ✅ return inside function
        }
    } else {
        // Update record: check all existing images except current record
        if (existingImageNames.some(img => img.name === fileName && img.id !== _Id)) {
            toastr.error("This image already exists in another record!");
            return false; // ✅ return inside function
        }
    }

    return true; // ✅ final return inside function
}
$('#txt_email').on('blur', function () {
    if (emailSent) return;
    let email = $(this).val().trim().toLowerCase();
    if (email === "") return;

    if (!isValidEmail(email)) {
        toastr.error("Please enter valid email");
        $('#otpBox').hide();
        emailSent = false; emailVerified = false;
        return;
    }

    if (_Id === 0 && existingEmails.includes(email)) {
        toastr.error("This email is already registered!");
        $('#otpBox').hide();
        emailSent = false; emailVerified = false;
        return;
    }

    if (email === lastVerifiedEmail) return;

    $.ajax({
        url: '/Home/SendOtp',
        type: 'POST',
        data: { email: email },
        success: function (res) {
            emailSent = true;
            emailVerified = false;
            toastr.info("OTP sent to " + email);

            $('#otpBox').show();
            $('#btnVerifyEmail').prop('disabled', false);
            $('#otpInput').prop('disabled', false).val('');
            $('#otpMessage').text('');

            startOtpTimer(600);
        },
        error: function () { toastr.error("Failed to send OTP"); }
    });
});

function startOtpTimer(seconds) {
    otpTimeLeft = seconds;
    clearInterval(otpTimer);
    updateOtpDisplay();

    otpTimer = setInterval(() => {
        otpTimeLeft--;
        if (otpTimeLeft <= 0) {
            clearInterval(otpTimer);
            $('#otpMessage').text("❌ OTP expired. Please resend OTP.");
            $('#otpInput').prop('disabled', true);
            $('#btnVerifyEmail').prop('disabled', true);
            emailSent = false;
        } else {
            updateOtpDisplay();
        }
    }, 1000);
}

function updateOtpDisplay() {
    let min = Math.floor(otpTimeLeft / 60);
    let sec = otpTimeLeft % 60;
    $('#otpMessage').text(`⏳ OTP valid for ${min}:${sec.toString().padStart(2, '0')} minutes`);
}

$('#btnVerifyEmail').on('click', function () {
    if (!emailSent) {
        toastr.warning("Please enter email first!");
        return;
    }

    let userOTP = $('#otpInput').val().trim();
    let email = $('#txt_email').val().trim();

    if (userOTP === "") {
        toastr.warning("Please enter OTP");
        return;
    }

    $.ajax({
        url: '/Home/VerifyOtp',
        type: 'POST',
        data: { email: email, otp: userOTP },
        success: function (res) {
            if (res.status === 200) {
                emailVerified = true;
                lastVerifiedEmail = email.toLowerCase();

                toastr.success("Email verified successfully!");
                $('#otpMessage').text("✅ Email Verified Successfully");

                $('#btnVerifyEmail').prop('disabled', true);
                $('#otpInput').prop('disabled', true);
                $('#txt_email').prop('readonly', true);

                clearInterval(otpTimer);
            } else {
                emailVerified = false;
                toastr.error(res.message);
                $('#otpMessage').text("❌ " + res.message);
            }
        },
        error: function () { toastr.error("Verification failed"); }
    });
});

function SaveStudentData() {
    _Id = parseInt($('#hdnId').val()) || 0; 
    console.log("Saving record Id:", _Id); 
    let name = $('#txt_name').val().trim();
    let email = $('#txt_email').val().trim();
    let number = $('#txt_number').val().trim();
    let message = $('#txt_message').val().trim();
    let file = $('#imageFile')[0].files[0];

    toastr.clear();
    if (name === "") return toastr.error("Name required");
    if (email === "") return toastr.error("Email required");
    if (!isValidEmail(email)) return toastr.error("Please enter valid email");
    if (!emailVerified) return toastr.error("Please verify your email first");
    if (!/^[0-9]{10}$/.test(number)) return toastr.error("Number must be 10 digits");
    if (message === "") return toastr.error("Message required");
    if (!ValidateImage() || !ValidateDuplicateImage(file)) return;

    if (file && $('#previewImg').attr('src').includes(file.name)) {
        file = null;
    }

    let formData = new FormData();
    formData.append("Id", _Id);
    formData.append("Name", name);
    formData.append("Email", email);
    formData.append("Number", number);
    formData.append("Message", message);
    if (file) formData.append("ImageFile", file);

    $.ajax({
        url: '/Home/SaveStudentData',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.statusCode === 200) {
                clearForm();
                GetInquiryDetails(0);
                toastr.success(res.message);
            } else {
                toastr.error(res.message);
            }
        },
        error: function (err) { toastr.error("Something went wrong"); console.error(err); }
    });
}

function GetInquiryDetails(id) {
    $.get('/Home/GetInquiryDetails', { Id: id }).done(function (data) {
        existingImageNames = data.map(v => ({
            id: v.id,
            name: (v.imageOriginalName || "").toLowerCase().trim()
        }));
        existingEmails = data.map(v => (v.email || "").toLowerCase().trim());

        let html = "";
        if (data.length > 0) {
            data.forEach((v, i) => {
                html += `<tr>
                    <td>${i + 1}</td>
                    <td>${v.name}</td>
                    <td>${v.email}</td>
                    <td>${v.number}</td>
                    <td>
                        <div class="zoom-container" style="position:relative;display:inline-block;width:120px;height:120px;">
                            <img src="/uploads/${v.imagePath}" class="tableImage" style="width:120px;height:120px;border-radius:5px;cursor:pointer;" onerror="this.src='/images/no-image.png'"/>
                            <div class="zoomLens" style="position:absolute;width:50px;height:50px;border:2px solid #000;display:none;opacity:0.4;background:white;pointer-events:none;"></div>
                        </div>
                    </td>
                    <td>${v.message}</td>
                    <td>
                        <button class="btn btn-warning" onclick="EditStudent(${v.id})">Edit</button>
                        <button class="btn btn-danger" onclick="DeleteStudent(${v.id})">Delete</button>
                    </td>
                </tr>`;
            });
        } else html = `<tr><td colspan="7">No record</td></tr>`;
        $('#tblRegistration').html(html);

        document.querySelectorAll('.tableImage').forEach(img => { applyZoom(img); applyClickModal(img); });
    });
}

function EditStudent(id) {
    _Id = id;
    $('#hdnId').val(id);

    $.get('/Home/GetStudentById', { Id: id }).done(function (data) {
        if (data) {
            $('#txt_name').val(data.name);
            $('#txt_email').val(data.email);
            $('#txt_number').val(data.number);
            $('#txt_message').val(data.message);
            emailVerified = true;
            $('#otpBox').hide();

            if (data.imagePath) {
                $('#previewImg').attr('src', '/uploads/' + data.imagePath).show();
            }

            $('#imageFile').val('');
        }
    });
}

function DeleteStudent(id) {
    if (!confirm("Are you sure you want to delete this record?")) return;
    $.post('/Home/DeleteStudent', { Id: id }).done(function (res) {
        if (res.statusCode === 200) { toastr.success(res.message); GetInquiryDetails(0); }
        else toastr.error(res.message);
    }).fail(() => { toastr.error("Delete failed"); });
}

function clearForm() {
    $('#txt_name,#txt_email,#txt_number,#txt_message,#imageFile').val('');
    $('#previewImg').hide();
    _Id = 0;
    $('#hdnId').val(0);
    emailVerified = false;
    emailSent = false;
    $('#otpBox').hide();
    $('#txt_email').prop('readonly', false);
}

function applyZoom(img) {
    const lens = img.parentElement.querySelector('.zoomLens');
    const result = document.createElement('div');
    result.className = 'zoomResult';
    result.style.width = '500px';
    result.style.height = '500px';
    result.style.border = '2px solid #000';
    result.style.backgroundRepeat = 'no-repeat';
    result.style.display = 'none';
    result.style.position = 'fixed';
    result.style.left = '150px';
    result.style.top = '50px';
    result.style.zIndex = 9999;
    result.style.backgroundSize = 'cover';
    result.style.borderRadius = '8px';
    result.style.boxShadow = '0 4px 15px rgba(0,0,0,0.3)';
    document.body.appendChild(result);

    img.addEventListener('mouseenter', () => {
        lens.style.display = 'block';
        result.style.display = 'block';
        result.style.backgroundImage = `url('${img.src}')`;
    });
    img.addEventListener('mouseleave', () => {
        lens.style.display = 'none';
        result.style.display = 'none';
    });
    img.addEventListener('mousemove', moveLens);
    lens.addEventListener('mousemove', moveLens);

    function moveLens(e) {
        e.preventDefault();
        const rect = img.getBoundingClientRect();
        let x = e.clientX - rect.left;
        let y = e.clientY - rect.top;
        x = Math.max(0, Math.min(x, img.width));
        y = Math.max(0, Math.min(y, img.height));
        const lensX = x - lens.offsetWidth / 2;
        const lensY = y - lens.offsetHeight / 2;
        lens.style.left = `${Math.max(0, Math.min(lensX, img.width - lens.offsetWidth))}px`;
        lens.style.top = `${Math.max(0, Math.min(lensY, img.height - lens.offsetHeight))}px`;

        const cx = result.offsetWidth / lens.offsetWidth;
        const cy = result.offsetHeight / lens.offsetHeight;
        result.style.backgroundSize = `${img.width * cx}px ${img.height * cy}px`;
        result.style.backgroundPosition = `-${lensX * cx}px -${lensY * cy}px`;
    }
}

function applyClickModal(img) {
    img.addEventListener('click', () => {
        const modal = document.getElementById('imgModal');
        const modalImg = document.getElementById('modalImg');
        modalImg.src = img.src;
        modal.style.display = 'flex';
    });
}

$(document).ready(function () {
    GetInquiryDetails(0);

    $('#txt_email').on('input', function () {
        let email = $(this).val().trim().toLowerCase();
        if (email !== lastVerifiedEmail) {
            emailVerified = false;
            emailSent = false;
            $('#otpBox').hide();
            $('#otpInput').val('').prop('disabled', false);
            $('#btnVerifyEmail').prop('disabled', false);
            $('#otpMessage').text('');
            toastr.warning("Email changed! Please verify again");
        }
    });

    window.SaveStudentData = SaveStudentData;
});