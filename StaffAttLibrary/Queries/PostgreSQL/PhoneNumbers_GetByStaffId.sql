SELECT pn."Id", pn."PhoneNumber"
FROM "PhoneNumbers" pn
JOIN "StaffPhoneNumbers" spn ON pn."Id" = spn."PhoneNumberId"
WHERE spn."StaffId" = @staffId;
