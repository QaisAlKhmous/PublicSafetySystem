var app = angular.module('myApp', ["ngRoute", 'ngTable']);


app.config(function ($routeProvider) {



    $routeProvider
       .when("/items", {
            templateUrl: '/Home/Items',
            controller: 'itemsCtrl'

       })

        .when("/matrices", {
            templateUrl: '/Home/Matrices',
            controller: 'matrixCtrl'

        })
        .when("/employees", {
            templateUrl: '/Home/Employees',
            controller: 'employeeCtrl'

        })
        .when("/addEmployee", {
            templateUrl: '/Home/AddEmployee',
            controller: 'addEmployeeCtrl'

        })
        .when("/editEmployee/:employeeId", {
            templateUrl: '/Home/AddEmployee',
            controller: 'addEmployeeCtrl'

        })
        
       


});



app.run(function ($rootScope, $location) {
    



})

app.factory('itemService', function ($http) {
    var baseUrl = '/Item'; // MVC controller base URL

    return {
        getItems: function () {
            
            return $http.get(baseUrl + '/GetItems');
        },
        getItemById: function (id) {
            return $http.get(baseUrl + '/GetItemById?id=' + id);
        },
        addItem: function (item) {
            return $http.post(baseUrl + '/AddItem', item);
        },
        updateItem: function (item) {
            return $http.post(baseUrl + '/UpdateItem', item);
        },
        deleteItem: function (id) {
            return $http.post(baseUrl + '/DeleteItem', { id: id });
        },
        increaseQuantity: function (id, quantity) {
            return $http.post(baseUrl + '/IncreaseQuantity', { id: id, quantity: quantity });
        }
    };
});



app.factory('matrixService', function ($http) {
    var baseUrl = '/Matrix'; // MVC controller base URL

    return {
        getAllMatrices: function () {

            return $http.get(baseUrl + '/GetAllMatrices');
        },
        getItemById: function (id) {
            return $http.get(baseUrl + '/GetItemById?id=' + id);
        },
        addItem: function (item) {
            return $http.post(baseUrl + '/AddItem', item);
        },
        updateItem: function (item) {
            return $http.post(baseUrl + '/UpdateItem', item);
        },
        deleteItem: function (id) {
            return $http.post(baseUrl + '/DeleteItem', { id: id });
        }
    };
});

app.factory('disposalService', function ($http) {
    var baseUrl = '/Disposal'; // MVC controller base URL

    return {
        addDisposal: function (disposal) {
            return $http.post(baseUrl + '/AddDisposal', disposal);
        }
    };
});

app.factory('employeeService', function ($http) {
    var baseUrl = '/Employee'; // MVC controller base URL

    return {
        getEmployeeById: function (id) {

            return $http.get(baseUrl + '/GetEmployee?Id=' + id);
        },
        addNewEmployee: function (employee) {
            return $http.post(baseUrl + '/AddNewEmployee' , employee);
        },
        updateEmployee: function (employee) {
            return $http.post(baseUrl + '/UpdateEmployee', employee);
        },
        getAllEmployees: function () {
            return $http.get(baseUrl + '/GetAllEmployees');
        },
        deleteEmployee: function (id) {
            return $http.post(baseUrl + '/ResignEmployee', { Id: id })
        }
    };
});

app.controller('itemsCtrl', function ($scope, NgTableParams, itemService, disposalService) {

    $scope.items = [];
    $scope.newItem = { Name: '', Description: '', Quantity: 0, AddedBy: 'Admin' }
    $scope.clicked = false;
    $scope.selectedItem = null;  
    $scope.addedQuantity = 0;
    $scope.disposal = {ItemId : null,Quantity: 0,DisposalDate: '',DisposalFormPath : '',CreatedBy: 'Admin',ApprovedBy: "Admin"}

    $scope.toastify = function (msg) {
        if (typeof Toastify !== "undefined") {
            Toastify({
                text: msg,
                duration: 3000,
                close: true,
                gravity: "bottom",
                position: "center",
                backgroundColor: "#4fbe87"
            }).showToast();
        }
    };

    $scope.loadItems = function () {
        itemService.getItems().then(function (response) {
            $scope.items = response.data;
            
            $scope.itemsTableParams.settings({ dataset: $scope.items });
            

        }, function (error) {
            console.error("Error loading items", error);
        });
    };

    $scope.itemsTableParams = new NgTableParams(
        {
            page: 1,            // start on first page
            count: 10,          // items per page
            filter: {},
            sorting: { Name: "asc" }// initial filter
        }
    );
    $scope.loadItems();

    $scope.addNewItem = function () {
        itemService.addItem($scope.newItem).then(function (response) {
            $scope.loadItems();
            $scope.toastify('تم اضافة المادة بنجاح');
        }, function (error) {
            console.error("Error loading items", error);
        });
    }

    $scope.confirmDelete = function (ItemId) {
        Swal.fire({
            title: 'هل انت متأكد من حذف المادة؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.deleteItem(ItemId);
        });
    }
   

    $scope.deleteItem = function (ItemId) {
        itemService.deleteItem(ItemId).then(function (response) {
            $scope.loadItems();
            $scope.toastify("تم حذف مادة بنجاح")
        }, function (error) {
            console.error("Error deleting items", error);
        });
    }

    $scope.changeSelectedItem = function (item) {
        $scope.selectedItem = item;
    }

    $scope.increaseQuantity = function () {
        console.log($scope.selectedItem);
        itemService.increaseQuantity($scope.selectedItem.ItemId,$scope.addedQuantity).then(function (response) {
            $scope.loadItems();
            $scope.toastify("تم زيادة رصيد مادة بنجاح")
        }, function (error) {
            console.error("Error deleting items", error);
        });
    }

    $(document).ready(function () {
        // Initialize disposal object if not already
        var scope = angular.element($('input[name="date"]')).scope();
        scope.$apply(function () {
            scope.disposal = scope.disposal || {};
            scope.disposal.DisposalDate = moment().format('YYYY-MM-DD'); // set default
        });

        $('input[name="date"]').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoUpdateInput: true, // input shows default
            startDate: moment(),   // today
            locale: { format: 'YYYY-MM-DD' }
        });

        $('input[name="date"]').on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('YYYY-MM-DD'));

            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.disposal.DisposalDate = picker.startDate.format('YYYY-MM-DD');
            });
        });

        $('input[name="date"]').on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.disposal.DisposalDate = '';
            });
        });
    });


    $scope.dispose = function () {
        $scope.disposal.ItemId = $scope.selectedItem.ItemId;
        disposalService.addDisposal($scope.disposal).then(function (response) {
            $scope.loadItems();
            $scope.toastify("تم اتلاف مادة بنجاح")
        }, function (error) {
            console.error("Error deleting items", error);
        })
    }
})

app.controller('matrixCtrl', function ($scope, NgTableParams, matrixService) {

    $scope.matrices = [];

    $scope.matricesByCategory = {}; 

    
   


    $scope.loadMatrices = function () {
        matrixService.getAllMatrices().then(function (response) {
            $scope.matrices = response.data;

            $scope.matrices.forEach(function (m) {
                var catName = m.Category;
                if (!$scope.matricesByCategory[catName]) {
                    $scope.matricesByCategory[catName] = [];
                }
                $scope.matricesByCategory[catName].push(m);
            });

            console.log($scope.matricesByCategory)

        }, function (error) {
            console.error("Error loading items", error);
        });
    };

  
    $scope.loadMatrices();


    $scope.isEdit = false;
})

app.controller('employeeCtrl', function ($scope, NgTableParams, employeeService, $location, itemService,$http) {

    $scope.employees = [];

    $scope.toastify = function (msg) {
        if (typeof Toastify !== "undefined") {
            Toastify({
                text: msg,
                duration: 3000,
                close: true,
                gravity: "bottom",
                position: "center",
                backgroundColor: "#4fbe87"
            }).showToast();
        }
    };

    $scope.getAllEmployees = function () {
        employeeService.getAllEmployees().then(function (response) {
            $scope.employees = response.data;

            $scope.employeesTableParams.settings({ dataset: $scope.employees });
        })
    }

    $scope.getAllEmployees();


    $scope.employeesTableParams = new NgTableParams(
        {
            page: 1,            // start on first page
            count: 10,          // items per page
            filter: {},
            sorting: { Name: "asc" }// initial filter
        }
    );


    $scope.GoToAddPage = function () {
        $location.path('/addEmployee');
    }

    // Handle Issue Exception

    $scope.issuance = { EmployeeId : null,IssuanceId: null, ItemId: null, Quantity: 1, Type: '', ExceptionReason: '', ExceptionFormPath: '' ,CreatedBy : 'Admin'};

    $scope.loadException = function (EmployeeId) {
        $scope.loadItems();
        $scope.issuance.Type = 'Exception';
        $scope.msg = 'تم اضافة استثناء مادة بنجاح'
        $scope.issuance.EmployeeId = EmployeeId;
    }

    $scope.loadDamaged = function (EmployeeId) {
        $scope.loadItems();
        $scope.issuance.Type = 'Damaged';
        $scope.msg = 'تم اضافة بدل تلف مادة بنجاح'
        $scope.issuance.EmployeeId = EmployeeId;
    }


    $scope.loadItems = function () {
        itemService.getItems().then((res) => {
            $scope.items = res.data;
        })
    }

    $scope.issueException = function () {
        $scope.exceptionForm.$setSubmitted();

        if (!$scope.issuance.ItemId) {

            console.warn("Form invalid");
            return;
        }

        $http.post('/Issuance/AddNewIssuance', $scope.issuance).then((res) => {
            $scope.toastify($scope.msg)
            $scope.issuance = { EmployeeId: null, IssuanceId: null, ItemId: null, Quantity: 1, Type: '', ExceptionReason: '', ExceptionFormPath: '', CreatedBy: 'Admin' };
            $scope.exceptionForm.$submitted = false;

        })
    }
    $scope.issueDamage = function () {
        $scope.damageForm.$setSubmitted();

        if (!$scope.issuance.ItemId) {

            console.warn("Form invalid");
            return;
        }

        $http.post('/Issuance/AddNewIssuance', $scope.issuance).then((res) => {
            $scope.toastify($scope.msg)
            $scope.issuance = { EmployeeId: null, IssuanceId: null, ItemId: null, Quantity: 1, Type: '', ExceptionReason: '', ExceptionFormPath: '', CreatedBy: 'Admin' };
            $scope.damageForm.$submitted = false;

        })
    }

    $scope.confirmDelete = function (EmployeeId) {
        Swal.fire({
            title: 'هل انت متأكد من تغيير حالة الموظف الى مستقيل؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.deleteEmployee(EmployeeId);
        });
    }

    $scope.deleteEmployee = function (EmployeeId) {
        employeeService.deleteEmployee(EmployeeId).then((res) => {
            $scope.toastify('تم تغيير حالة الموظف الى مستقيل')
        })
    }
   
})

app.controller('addEmployeeCtrl', function ($scope, NgTableParams, employeeService, $location, $http, $timeout, $routeParams) {

    $scope.toastify = function (msg) {
        if (typeof Toastify !== "undefined") {
            Toastify({
                text: msg,
                duration: 3000,
                close: true,
                gravity: "bottom",
                position: "center",
                backgroundColor: "#4fbe87"
            }).showToast();
        }
    };

    $scope.GoToListPage = function () {
        $location.path('/employees');
    }

    $(document).ready(function () {
        // Initialize disposal object if not already
        var scope = angular.element($('input[name="date"]')).scope();
        scope.$apply(function () {
            scope.employee.EmploymentDate = scope.employee.EmploymentDate || {};
            scope.employee.EmploymentDate = moment().format('YYYY-MM-DD'); // set default
        });

        $('input[name="date"]').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoUpdateInput: true, // input shows default
            startDate: moment(),   // today
            locale: { format: 'YYYY-MM-DD' }
        });

        $('input[name="date"]').on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('YYYY-MM-DD'));

            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.employee.EmploymentDate = picker.startDate.format('YYYY-MM-DD');
            });
        });

        $('input[name="date"]').on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.employee.EmploymentDate = '';
            });
        });
    });


   

    $scope.loadJobTitles = function () {
        $http.get('/JobTitle/GetAllJobTitles')
            .then((res) => {
                $scope.JobTitles = res.data;
            }).catch((rej) => console.log(rej))
    }


    $scope.loadDepartments = function () {
        $http.get('/Department/GetAllDepartments')
            .then((res) => {
                $scope.Departments = res.data;
            }).catch((rej) => console.log(rej))
    }

    $scope.loadSections = function () {
        $http.get('/Section/GetAllSections')
            .then((res) => {
                $scope.Sections = res.data;
            }).catch((rej) => console.log(rej))
    }

    $scope.loadCategories = function () {
        $http.get('/Category/GetAllCategories')
            .then((res) => {
                console.log(res);
                $scope.Categories = res.data;
            }).catch((rej) => console.log(rej))
    }


    $scope.loadCategories();
    $scope.loadSections();
    $scope.loadDepartments();
    $scope.loadJobTitles();

    $scope.employee = {
        Name: '', Email: '', Phone: '', IsIntern: false, Notes: '', WorkLocation: 'Amman',
        HealthInsuranceFile: '', DepartmentId: null, SectionId: null, JobTitleId: null, CategoryId: null,
        EmploymentDate: '',Notes: ''
    }

    $scope.updateEmployeeId = $routeParams.employeeId;



    $scope.checkIfUpdate = function () {
        if ($scope.updateEmployeeId) {
            console.log("hi");
            employeeService.getEmployeeById($scope.updateEmployeeId)
                .then((res) => {
                  
                   $scope.employee = res.data;

                })
        }
    }
    $scope.checkIfUpdate();

    $scope.saveEmployee = function () {
        $scope.employeeForm.$setSubmitted();

        if ($scope.employeeForm.$invalid || !$scope.employee.JobTitleId || !$scope.employee.DepartmentId || !$scope.employee.SectionId) {

            console.warn("Form invalid");
            return;
        }


        if ($scope.updateEmployeeId ) {
            employeeService.updateEmployee($scope.employee)
                .then((res) => {
                    $scope.toastify('تم تعديل بيانات الموظف بنجاح');
                }).catch((rej) => console.log(rej))
        }
        else {
            employeeService.addNewEmployee($scope.employee)
                .then((res) => {
                    $scope.toastify('تم اضافة موظف بنجاح');
                    $location.path('/editEmployee/' + res.data.id);
                }).catch((rej) => console.log(rej))

        }

       

       
      
    }

    $scope.downloadHealthInsurance = function (userId) {
        $http.get('/Upload/DownloadHealthInsuranceFile', { params: { userId: userId } })
            .then(function (response) {
                if (response.data.success) {

                    window.open(response.data.fileUrl, '_blank');
                    toastr.success("File is ready for download.");
                } else {
                    toastr.error(response.data.message);
                }
            })
            .catch(function (err) {
                toastr.error("حدث خطأ أثناء تحميل الملف.");
                console.error(err);
            });
    };


    $timeout(function () {
        Dropzone.autoDiscover = false;

        $scope.myDropzone = new Dropzone("#dropzone", {
            paramName: "file",
            addRemoveLinks: true,
            maxFiles: 1,
            acceptedFiles: '.jpeg,.jpg,.png,.gif,.pdf,.doc,.msg',
            dictDefaultMessage: '<strong>Upload a Doc</strong><br/><small>From your computer</small>',
            url: "/Upload/UploadFile",

            init: function () {
                var dz = this;

                this.on("success", function (file, response) {
                    $scope.$applyAsync(function () {
                        $scope.employee.HealthInsuranceFile = '/Uploads/' + response.tempId;
                    });
                });

                // If updating employee, load the existing file now
                if ($scope.employee && $scope.employee.HealthInsuranceFile) {
                    var existingFileName = $scope.employee.HealthInsuranceFile.split('/').pop();
                    var existingFileUrl = $scope.employee.HealthInsuranceFile;

                    var mockFile = { name: existingFileName, size: 12345, accepted: true };

                    dz.emit("addedfile", mockFile);
                    dz.emit("complete", mockFile);

                    // For images, show thumbnail
                    if (/\.(jpg|jpeg|png|gif)$/i.test(mockFile.name)) {
                        dz.emit("thumbnail", mockFile, existingFileUrl);
                    }

                    // Add clickable link to open file
                    mockFile.previewElement.querySelector("a.dz-remove").addEventListener("click", function (e) {
                        window.open(existingFileUrl, "_blank");
                    });

                    dz.files.push(mockFile);
                }
            }
        });
    },200);



  


    
})